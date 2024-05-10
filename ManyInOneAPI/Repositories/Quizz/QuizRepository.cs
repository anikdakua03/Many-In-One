using ExcelDataReader;
using ManyInOneAPI.Data;
using ManyInOneAPI.Infrastructure;
using ManyInOneAPI.Infrastructure.Shared;
using ManyInOneAPI.Models.Quizz;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace ManyInOneAPI.Repositories.Quizz
{
    public class QuizRepository : IQuizRepository
    {
        private readonly ManyInOneDbContext _dbContext;
        //private readonly ManyInOnePgDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QuizRepository(ManyInOneDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<List<QuestionResponse>>> GetQuestions()
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue("Id");

            var userExists = _dbContext.Users.Find(userId!);

            if (userExists is null)
            {
                return Result<List<QuestionResponse>>.Failure(Error.NotFound("Not Found", "User doesn't exists or Invalid user."));
            }

            var categoryList = await _dbContext.Categories.Where(a => a.UserId == Guid.Parse(userExists.Id)).ToListAsync();

            if (categoryList.Count == 0)
            {
                return Result<List<QuestionResponse>>.Failure(Error.NotFound("Not Found", "Category doesn't exists."));
            }

            var allQss = new List<Question>();

            categoryList.ForEach(a => allQss.AddRange(
                _dbContext.Questions.Include(a => a.Options)
                                            .Where(b => b.CategoryId == a.CategoryId).ToList()));

            var allQssRes = new List<QuestionResponse>();

            allQss.ForEach(a => allQssRes.Add(new QuestionResponse()
            {
                QuestionId = a.QuestionId,
                QuestionText = a.QuestionText,
                QuestionImageLink = a.QuestionImageLink!,
                QuestionType = a.QuestionType.ToString(),
                QuestionLevel = a.QuestionLevel.ToString(),
                QuestionTags = a.QuestionTags,
                CategoryName = _dbContext.Categories.FindAsync(a.CategoryId).Result!.CategoryName,
                Options = a.Options
            }));

            return Result<List<QuestionResponse>>.Success(allQssRes);
        }

        public async Task<Result<string>> AddQuestion(AddQuestionRequest qReq, CancellationToken cancellationToken)
        {
            // find category id
            var category = await _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(a => a.CategoryName == qReq.CategoryName, cancellationToken);

            if (category is null)
            {
                return Result<string>.Failure(Error.NotFound("NotFound", $"Create \"{qReq.CategoryName}\" category first."));
            }

            var opreq = qReq.Options;

            if (opreq.Count < 2)
            {
                return Result<string>.Failure(Error.Validation("Invalid", "Question with minimum 2 option required."));
            }
            var QType = Enum.Parse<QuestionType>(qReq.QuestionType.ToUpper());
            var QLvl = Enum.Parse<QuestionLevel>(qReq.QuestionLevel.ToUpper());

            // prepare question 
            var qs = new Question()
            {
                QuestionText = qReq.QuestionText,
                QuestionImageLink = string.IsNullOrWhiteSpace(qReq.QuestionImageLink) ?
                                    "NA" :
                                    qReq.QuestionImageLink,
                QuestionType = QType,
                QuestionLevel = QLvl,
                QuestionTags = qReq.QuestionTags,
                CategoryId = category!.CategoryId,
                Options = opreq.Select(a => new OptionsWithAnswer()
                {
                    OptionValue = a.OptionValue,
                    AnswerExplanation = string.IsNullOrWhiteSpace(a.AnswerExplanation) ?
                                        "No Explanation available." :
                                        a.AnswerExplanation,
                    IsAnswer = a.IsAnswer,
                }).ToList()
            };

            await _dbContext.Questions.AddAsync(qs, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Question added successfully. ");
        }

        public async Task<Result<string>> AddQuestions(CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue("Id");

            var userExists = _dbContext.Users.Find(userId!);

            if (userExists is null)
            {
                return Result<string>.Failure(Error.NotFound("Not Found", "User doesn't exists or Invalid user to add questions."));
            }

            var file = _httpContextAccessor.HttpContext!.Request.Form.Files[0];

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            bool check = IsFileAllowed(file);

            if (file is null || file.Length == 0 || !check)
            {
                return Result<string>.Failure(Error.Validation("Invalid", "Invalid file."));
            }

            var uploadFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Upload";
            
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var filePath = Path.Combine(uploadFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // list of qs initialization
            List<Question> allQss = new List<Question>();
            int rowCounter = 0;

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Delete))
            {
                try
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            bool isHeader = false;

                            while (reader.Read())
                            {

                                if (!isHeader)
                                {
                                    isHeader = true;
                                    continue;
                                }
                                rowCounter++;

                                var qs = new Question() { };

                                var cateGoryObj = reader.GetValue(0);
                                var categoryName = cateGoryObj is null ? "Default Category" : cateGoryObj.ToString()!.Trim();

                                var category = await _dbContext.Categories.FirstOrDefaultAsync(a => a.CategoryName == categoryName);

                                if (category is null)
                                {
                                    var newCate = new Category()
                                    {
                                        CategoryId = Guid.NewGuid(),
                                        CategoryName = categoryName,
                                        Description = "No description provided.",
                                        UserId = Guid.Parse(userExists.Id)
                                    };

                                    _dbContext.Categories.Add(newCate);
                                    await _dbContext.SaveChangesAsync();

                                    qs.CategoryId = newCate.CategoryId;
                                }
                                else
                                {
                                    qs.CategoryId = category.CategoryId;
                                }


                                var qsObj = reader.GetValue(1);
                                if (qsObj is null)
                                {
                                    // remove the file for clearing
                                    File.Delete(filePath);

                                    return Result<string>.Failure(Error.Validation("Invalid", "Question must not be empty."));
                                }

                                qs.QuestionText = qsObj.ToString()!.Trim();

                                #region Options

                                var opList = new List<string>();

                                var op1 = reader.GetValue(2);
                                var op2 = reader.GetValue(3);
                                var op3 = reader.GetValue(4);
                                var op4 = reader.GetValue(5);
                                var op5 = reader.GetValue(6);

                                if (op1 is null || op2 is null)
                                {
                                    // remove the file for clearing
                                    File.Delete(filePath);

                                    return Result<string>.Failure(Error.Validation("Invalid", "Option 1 or Option 2 must not be empty."));
                                }
                                opList.Add(op1.ToString()!.Trim());
                                opList.Add(op2.ToString()!.Trim());

                                string opStr3 = op3 is null ? null! : op3.ToString()!.Trim();
                                if (!string.IsNullOrWhiteSpace(opStr3))
                                {
                                    opList.Add(opStr3);
                                }

                                string opStr4 = op4 is null ? null! : op4.ToString()!.Trim();
                                if (!string.IsNullOrWhiteSpace(opStr4))
                                {
                                    opList.Add(opStr4);
                                }

                                string opStr5 = op5 is null ? null! : op5.ToString()!.Trim();
                                if (!string.IsNullOrWhiteSpace(opStr5))
                                {
                                    opList.Add(opStr5);
                                }

                                #endregion

                                #region Answer / Answers

                                var ansList = new List<string>();

                                var ans1 = reader.GetValue(7);
                                var ans2 = reader.GetValue(8);
                                var ans3 = reader.GetValue(9);
                                var ans4 = reader.GetValue(10);
                                var ans5 = reader.GetValue(11);

                                string ansStr1 = ans1 is null ? null! : ans1.ToString()!.Trim();
                                if (!string.IsNullOrWhiteSpace(ansStr1))
                                {
                                    ansList.Add(ansStr1);
                                }

                                string ansStr2 = ans2 is null ? null! : ans2.ToString()!.Trim();
                                if (!string.IsNullOrWhiteSpace(ansStr2))
                                {
                                    ansList.Add(ansStr2);
                                }

                                string ansStr3 = ans3 is null ? null! : ans3.ToString()!.Trim();
                                if (!string.IsNullOrWhiteSpace(ansStr3))
                                {
                                    ansList.Add(ansStr3);
                                }

                                string ansStr4 = ans4 is null ? null! : ans4.ToString()!.Trim();
                                if (!string.IsNullOrWhiteSpace(ansStr4))
                                {
                                    ansList.Add(ansStr4);
                                }

                                string ansStr5 = ans5 is null ? null! : ans5.ToString()!.Trim();
                                if (!string.IsNullOrWhiteSpace(ansStr5))
                                {
                                    ansList.Add(ansStr5);
                                }

                                #endregion

                                #region Options and answer validation

                                var isValid = opList.Intersect(ansList).Any();

                                if (!isValid)
                                {
                                    // remove the file for clearing
                                    File.Delete(filePath);

                                    return Result<string>.Failure(Error.Validation("Invalid", "Answer is not in the given options."));
                                }

                                var opsWithAns = new List<OptionsWithAnswer>();

                                foreach (var op in opList)
                                {
                                    var owa = new OptionsWithAnswer() { };
                                    if (ansList.Contains(op))
                                    {
                                        owa.IsAnswer = true;
                                        var expla = reader.GetValue(12);
                                        owa.AnswerExplanation = expla is null ? "No explanation available." : expla.ToString()!.Trim();
                                    }
                                    owa.OptionValue = op;

                                    opsWithAns.Add(owa);
                                }

                                qs.Options = opsWithAns;

                                #endregion


                                var qsType = reader.GetValue(13).ToString();
                                qs.QuestionType = GetQuestionType(qsType!);

                                var qsLevel = reader.GetValue(14).ToString();
                                qs.QuestionLevel = GetQuestionLevel(qsLevel!);

                                var imgLink = reader.GetValue(15);
                                qs.QuestionImageLink = imgLink is null ? "NA" : imgLink.ToString();

                                var qsTags = reader.GetValue(16);
                                var tags = qsTags is null ? new List<string>() : qsTags.ToString()!.Split(',').ToList();
                                qs.QuestionTags = tags;

                                allQss.Add(qs);
                            }

                        }
                        while (reader.NextResult());
                    }
                }
                catch (Exception ex)
                {
                    // remove the file for clearing
                    File.Delete(filePath);

                    return Result<string>.Failure(Error.Validation("Invalid", $"Invalid or corrupted file , please upload correct file. {ex.Message} "));
                }
            }

            if (allQss.Count == 0)
            {
                // remove the file for clearing
                File.Delete(filePath);

                return Result<string>.Failure(Error.NotFound("Not Found", "No question found in the file. "));
            }

            int rowCount = 2; // as pers row starts in excel

            foreach (var qs in allQss)
            {
                var dupQs = await _dbContext.Questions.FirstOrDefaultAsync(a => a.QuestionText == qs.QuestionText);

                if (dupQs is not null)
                {
                    // remove the file for clearing
                    File.Delete(filePath);

                    return Result<string>.Failure(Error.Conflict("Conflict", $"Duplicate question found at row number {rowCount} in the file. "));
                }

                rowCount++;
            }

            await _dbContext.Questions.AddRangeAsync(allQss);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // clearing the file after processing
            File.Delete(filePath);

            return Result<string>.Success($"{rowCounter} question / questions added successfully. ");
        }

        public async Task<Result<string>> UpdateQuestion(UpdateQuestionRequest qReq, CancellationToken cancellationToken)
        {
            var oldQs = await _dbContext.Questions.FindAsync(Guid.Parse(qReq.QuestionId), cancellationToken);

            if (oldQs is null)
            {
                return Result<string>.Failure(Error.NotFound("NotFound", "Question doesn't exists."));
            }

            var opreq = qReq.Options;

            if (opreq.Count < 2)
            {
                return Result<string>.Failure(Error.Validation("Invalid", "Question with minimum 2 option required."));
            }

            // delete previous options
            int rows = await _dbContext.Options.Where(a => a.QuestionId == oldQs.QuestionId).ExecuteDeleteAsync(cancellationToken);

            var QType = Enum.Parse<QuestionType>(qReq.QuestionType.ToUpper());
            var QLvl = Enum.Parse<QuestionLevel>(qReq.QuestionLevel.ToUpper());

            // update question 
            oldQs.QuestionText = qReq.QuestionText;
            oldQs.QuestionImageLink = string.IsNullOrWhiteSpace(qReq.QuestionImageLink) ?
                                "NA" : qReq.QuestionImageLink;
            oldQs.QuestionType = QType;
            oldQs.QuestionLevel = QLvl;
            oldQs.QuestionTags = qReq.QuestionTags;
            oldQs.Options = opreq.Select(a => new OptionsWithAnswer()
            {
                OptionValue = a.OptionValue,
                AnswerExplanation = string.IsNullOrWhiteSpace(a.AnswerExplanation) ?
                                    "No Explanation available." : a.AnswerExplanation,
                IsAnswer = a.IsAnswer,
            }).ToList();

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Question updated successfully.");
        }

        public async Task<Result<string>> RemoveQuestion(string questionId, CancellationToken cancellationToken)
        {
            var oldQs = await _dbContext.Questions.FindAsync(Guid.Parse(questionId), cancellationToken);

            if (oldQs is null)
            {
                return Result<string>.Failure(Error.NotFound("NotFound", "Question doesn't exists."));
            }

            _dbContext.Questions.Remove(oldQs);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Question removed successfully. ");
        }


        public Result<CustomCategoryResponse> GetCategories()
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue("Id");

            var userExists = _dbContext.Users.Find(userId!);

            if (userExists is null)
            {
                return Result<CustomCategoryResponse>.Failure(Error.NotFound("Not Found", "User doesn't exists or Invalid user."));
            }

            var res = _dbContext.Categories.Where(a => a.UserId == Guid.Parse(userExists.Id)).ToList();

            if (res.Count == 0)
            {
                return Result<CustomCategoryResponse>.Failure(Error.NotFound("Not Found", "Not categories are there."));
            }

            var categories = new CustomCategoryResponse();

            var listOfCategory = res.Select(a => new SingleCategory
            {
                CategoryId = a.CategoryId,
                CategoryName = a.CategoryName,
                Description = a.Description,
                QuestionCount = _dbContext.Questions.Where(b => b.CategoryId == a.CategoryId).Count()
            }).ToList();

            categories.Categories = listOfCategory;

            return Result<CustomCategoryResponse>.Success(categories);
        }

        public async Task<Result<string>> AddCategory(AddCategoryRequest catReq, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue("Id");

            var userExists = _dbContext.Users.Find(userId!);

            if (userExists is null)
            {
                return Result<string>.Failure(Error.NotFound("Not Found", "User doesn't exists or Invalid user to add question."));
            }

            var newCategory = new Category()
            {
                CategoryName = catReq.CategoryName,
                Description = string.IsNullOrEmpty(catReq.Description) ? "No description provided." : catReq.Description,
                UserId = Guid.Parse(userExists.Id)
            };

            await _dbContext.Categories.AddAsync(newCategory, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Category added successfully. ");
        }

        public async Task<Result<string>> UpdateCategory(UpdateCategoryName updateCategoryName, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue("Id");

            var userExists = _dbContext.Users.Find(userId!);

            if (userExists is null)
            {
                return Result<string>.Failure(Error.NotFound("Not Found", "User doesn't exists or Invalid user."));
            }

            var oldCategory = await _dbContext.Categories.FirstOrDefaultAsync(a => a.CategoryName == updateCategoryName.CategoryName && a.UserId == Guid.Parse(userExists.Id), cancellationToken);

            if (oldCategory is null)
            {
                return Result<string>.Failure(Error.NotFound("NotFound", "Category doesn't exists."));
            }

            oldCategory.CategoryName = updateCategoryName.NewCategoryName;
            oldCategory.Description = string.IsNullOrEmpty(updateCategoryName.Description) ? "No description provided." : updateCategoryName.Description;

            _dbContext.Categories.Update(oldCategory);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Category updated successfully. ");
        }

        public async Task<Result<string>> RemoveCategory(string categoryId, CancellationToken cancellationToken)
        {
            var oldCategory = await _dbContext.Categories.FindAsync(Guid.Parse(categoryId), cancellationToken);

            if (oldCategory is null)
            {
                return Result<string>.Failure(Error.NotFound("Not Found", "Category doesn't exists."));
            }

            _dbContext.Categories.Remove(oldCategory);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Category and it's all question removed successfully. ");
        }

        #region Options related if neede later
        public async Task<Result<string>> AddOption(AddOptionRequest addOpReq, CancellationToken cancellationToken)
        {
            var c = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);

            if (!string.IsNullOrEmpty(c))
            {
                return Result<string>.Failure(Error.Validation("Invalid User", "Cannot add question for non application user."));
            }

            var checkQs = await _dbContext.Questions.FirstOrDefaultAsync(a => a.QuestionText == addOpReq.QuestionText, cancellationToken);

            if (checkQs is null)
            {
                return Result<string>.Failure(Error.NotFound("Not Found", "No question found for this question name to add option."));
            }

            var newOption = new OptionsWithAnswer()
            {
                OptionValue = addOpReq.OptionValue,
                AnswerExplanation = addOpReq.AnswerExplanation,
                IsAnswer = addOpReq.IsAnswer,
                QuestionId = checkQs.QuestionId
            };

            await _dbContext.Options.AddAsync(newOption, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Option added successfully. ");
        }

        public async Task<Result<string>> UpdateOption(UpdateOptionRequest updateOpReq, CancellationToken cancellationToken)
        {
            var c = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);

            if (!string.IsNullOrEmpty(c))
            {
                return Result<string>.Failure(Error.Validation("Invalid User", "Cannot add question for non application user."));
            }

            var oldOption = await _dbContext.Options.FirstOrDefaultAsync(a => a.OptionValue == updateOpReq.OptionValue, cancellationToken);

            if (oldOption is null)
            {
                return Result<string>.Failure(Error.NotFound("Not Found", "Option doesn't exists."));
            }

            oldOption.OptionValue = updateOpReq.NewOptionValue;
            oldOption.AnswerExplanation = updateOpReq.AnswerExplanation;
            oldOption.IsAnswer = updateOpReq.IsAnswer;

            _dbContext.Options.Update(oldOption);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Option updated successfully. ");
        }

        public async Task<Result<string>> RemoveOption(string optionId, CancellationToken cancellationToken)
        {
            var oldOption = await _dbContext.Options.FindAsync(Guid.Parse(optionId), cancellationToken);

            if (oldOption is null)
            {
                return Result<string>.Failure(Error.NotFound("Not Found", "Category doesn't exists."));
            }

            var qss = await _dbContext.Questions.FirstOrDefaultAsync(a => a.QuestionId == oldOption.QuestionId, cancellationToken);

            if (qss is not null && qss.Options.Count == 2)
            {
                return Result<string>.Failure(Error.Validation("Invalid", "A question must have two or more options, hence cannot delete the option."));
            }

            _dbContext.Options.Remove(oldOption);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Option removed successfully.");
        }

        #endregion



        public async Task<Result<List<ClientCustomQuizResponse>>> MakeQuiz(QuizMakerRequest quizMakerRequest, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue("Id");

            var userExists = _dbContext.Users.Find(userId!);

            if (userExists is null)
            {
                return Result<List<ClientCustomQuizResponse>>.Failure(Error.NotFound("Not Found", "User doesn't exists or Invalid user to create a quiz."));
            }

            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(a => a.CategoryId == Guid.Parse(quizMakerRequest.CategoryId) && a.UserId == Guid.Parse(userExists.Id), cancellationToken);

            if (category is null)
            {
                return Result<List<ClientCustomQuizResponse>>.Failure(Error.NotFound("Not Found", "Category doesn't exists."));
            }

            var allQss = await _dbContext.Questions.Where(a => a.CategoryId == category!.CategoryId)
                                                              .Include(a => a.Options)
                                                              .ToListAsync(cancellationToken);

            if (allQss.Count < 5)
            {
                return Result<List<ClientCustomQuizResponse>>.Failure(Error.Validation("Invalid", "Not enough question in this category to make a quiz, please try with other option."));
            }

            allQss.Shuffle();

            allQss = allQss.Take(quizMakerRequest.QuestionCount).ToList();
            //Enum.TryParse(quizMakerRequest.QuestionLevel.ToUpper(), out QuestionLevel qLvl);
            //Enum.TryParse(quizMakerRequest.QuestionType.ToUpper(), out QuestionType qtype);

            var enumQType = Enum.Parse<QuestionType>(quizMakerRequest.QuestionType.ToUpper());
            var enumLvl = Enum.Parse<QuestionLevel>(quizMakerRequest.QuestionLevel.ToUpper());

            // if qs level is other than any then can filter
            if (enumLvl != QuestionLevel.ANY)
            {
                allQss = allQss.Where(a => a.QuestionLevel == enumLvl).ToList();
            }

            // if qs type is other than any then can filter
            if (enumQType != QuestionType.ANY)
            {
                allQss = allQss.Where(a => a.QuestionType == enumQType).ToList();
            }

            var final = UserQuizResToClientRes(allQss);

            return Result<List<ClientCustomQuizResponse>>.Success(final);
        }

        public async Task<Result<QuizResultResponse>> GetQuizScore(UserAnswerRequest userAnswerRequest, CancellationToken cancellationToken)
        {
            int totalQs = userAnswerRequest.UserQsAnswers.Count;
            if (totalQs == 0)
            {
                return Result<QuizResultResponse>.Failure(Error.NotFound("Not found", "No answers are there to calculate."));
            }

            var quizRes = new QuizResultResponse();
            var ansDetails = new List<CorrectAnswer>();
            int totalCorrect = 0;
            foreach (var item in userAnswerRequest.UserQsAnswers)
            {
                var userAns = item.SelectedAnswer;

                var corrects = await _dbContext.Options
                                                    .Where(a => a.QuestionId == Guid.Parse(item.QuestionId))
                                                    .Where(b => b.IsAnswer == true)
                                                    .ToListAsync();

                List<string> ansExplanations = new List<string>() { };

                if (corrects.Count > 1 && corrects.Count == userAns.Count)
                {
                    // for multiple option correct
                    int selectedCorrectOption = 0;
                    foreach (var correct in corrects)
                    {
                        if (userAns.Contains(correct.OptionValue))
                        {
                            selectedCorrectOption++;
                        }
                        ansExplanations.Add(correct.AnswerExplanation!);
                    }
                    totalCorrect += selectedCorrectOption == corrects.Count ? 1 : 0;
                }
                else
                {
                    // only will accept those if has single answer given
                    if (userAns.Count == 1 && corrects.Count > 0 && userAns[0] == corrects[0].OptionValue)
                    {
                        totalCorrect++;
                    }
                    else
                    {
                        //totalCorrect--;
                    }
                    ansExplanations.Add(corrects[0]!.AnswerExplanation!);
                }

                ansDetails.Add(new CorrectAnswer() { QuestionId = item.QuestionId, AnswerExplanation = ansExplanations });
            }
            quizRes.TotalCorrect = totalCorrect;
            quizRes.TotalQs = totalQs;
            quizRes.Percentage = (totalCorrect * 100) / totalQs;
            quizRes.HasPassed = quizRes.Percentage >= 80 ? true : false;
            quizRes.Results = ansDetails;

            return Result<QuizResultResponse>.Success(quizRes);
        }


        private List<ClientCustomQuizResponse> UserQuizResToClientRes(List<Question> qss)
        {
            var result = new List<ClientCustomQuizResponse>();

            for (int i = 0; i < qss.Count; i++)
            {
                var currentCat = _dbContext.Categories.FirstOrDefault(a => a.CategoryId == qss[i].CategoryId);

                var cqr = new ClientCustomQuizResponse()
                {
                    QuestionId = qss[i].QuestionId,
                    QuestionText = qss[i].QuestionText,
                    QuestionImageLink = qss[i].QuestionImageLink,
                    QuestionType = qss[i].QuestionType.ToString(),
                    QuestionLevel = qss[i].QuestionLevel.ToString(),
                    QuestionTags = qss[i].QuestionTags,
                    CategoryName = currentCat!.CategoryName,
                    Options = qss[i].Options.Select(a => new OptionResponse()
                    {
                        OptionId = a.OptionId,
                        OptionValue = a.OptionValue
                    }).ToList(),

                };

                result.Add(cqr);
            }

            return result;
        }

        private bool IsFileAllowed(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            string[] allowedExt = { ".xlsx", ".xls" };
            string[] allowedContentType = { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel" };
            int permissibaleSize = 1024 * 1024;
            return allowedExt.Contains(extension) && allowedContentType.Contains(file.ContentType) && file.Length <= permissibaleSize;
        }

        private QuestionType GetQuestionType(string qsType)
        {
            switch (qsType)
            {
                case "Multiple Correct":
                    return QuestionType.MULTIPLE_CORRECT;
                case "Only One Correct":
                    return QuestionType.MULTIPLE;
                case "True / False":
                    return QuestionType.BOOLEAN;
                default:
                    return QuestionType.ANY;
            }
        }

        private QuestionLevel GetQuestionLevel(string qsLevel)
        {
            switch (qsLevel)
            {
                case "Easy":
                    return QuestionLevel.EASY;
                case "Medium":
                    return QuestionLevel.MEDIUM;
                case "Hard":
                    return QuestionLevel.HARD;
                default:
                    return QuestionLevel.ANY;
            }
        }
    }
}
