# Many In One

It is a fully functional web application written in .NET and Angular frameworks.

## Table of Content : 

- [Many In One](#many-in-one)
  - [Table of Content](#table-of-content-)
  - [Tech Stack](#tech-stack-)
  - [Available features with some screenshots](#available-features-with-some-screenshots-)
  - [Things used](#things-used-)
  - [Deployment methods used](#deployment-methods-used-)
  - [Steps to run locally](#steps-to-run-locally-)
  - [🔗Ref Links](#ref-links-)
  - [Things can be added](#things-can-be-added-)


## Tech Stack :

**Client :** Angular 17, TailwindCSS

**Server :** ASP.NET Core Web API [.NET 8]

**Database :** MSSQL

## Available features with some screenshots :

1. Secure Login / Registration with two factor authentication .
    a. login 
    b. registration
    
2. User profile and other details maangement
    a. enable two factor
    b. disable two factor
    c. delete all user data
    d. quiz management
        i. add / update / delete category
        ii. add / update / delete question
        iii. add multiple questions after filling the provided question template and upload

3. CRUD operation 
    - like simple payment card addition / deleteion / updation

4. Use some generative AI tools 
    a. provide your query and get answer to it
    b. provide image and its related question and get answer
    c. summarize long text
    d. describe your thoughts in a text to generate image
    e. provide some text to convert to audio
    f. conversation like qna with gen AI

5. Quiz maker and play quiz 
    a. can create quiz of any available categories and play that.
    b. otherwise first create your own set of quiz questions and then play with it.

6. Check some clash of clan details
    a. search player and see details.
    b. search clan by tag or search clans with some filters 


## Things used :

1. In API :
    - Inegrated JWT token with cookies.
    - Tried to use **Result Pattern**.
    - Used Global Exception handling.
    - Using of Entity FrameworkCore for querying from database.
    - Used encryption for one of the feature for quiz.
    - Integrated mail service for sending mails. 
    - Applied rate limitting.

2. In Angular side :
    - Implemented with some mixed conventions.

## Deployment methods used :

> Tried to deploy it free on the with all existing things used . So after some finding I used **Google Cloud** as cloud service provider.
1. First of all containerized both dotnet and angular application.
2. For database, created a VM , then run **Postgres** in a containerized format.
3. Next Added those two image in the CGP's **Artifact Registry**, so that I can use those two image for creating a service which will be running on **Cloud Run**.

> There are steps where we can set up our environment variable in cloud run.

## Steps to run locally :

1. Get all the files.
2. Create enviroments folder inside **src** folder, then create **environment.development.ts** and **environment.ts** file.
3. Next insert these credentials and update with your credentials.
```ts
export const environment = {
    production: true,
    apiBaseUrl: "API_URL", 
    clientId: "FOR_GOOGLE_LOGIN_GET_ID_FROM_GCLOUD",
    cryptoSecretKey: "IF_WANT_TO_USE_CRYPTOGRAPHY_FOR_HIDING_SENSITIVE"
};
```
4. Now for web api, can all settings in **appsettings.json** file or can create **dotnet secrets** to store all the credentials.
Command to create dotnet secrets :
    a. To initialize secrets
    ```cs
    dotnet user-secrets init --project .\ManyInOneAPI
    ```
    b. Next setting secrets as key value
    ```cs
    dotnet user-secrets set --project .\ManyInOneAPI "GenAI:API_KEY" "YOUR_API_KEY"
    ```
    c. Next to view the secrets
    ```cs
    dotnet user-secrets list --project .\ManyInOneAPI
    ```
 > NOTE : If you need to run all working , you need to set all like or can be modified accordingly.
 ```json
    "MailConfig:Port": "",
    "MailConfig:Password": "",
    "MailConfig:Host": "",
    "MailConfig:FromEmail": "",
    "MailConfig:DisplayName": "",
    "GenAI:ProVisionUrl": "",
    "GenAI:GenAIBaseUrl": "",
    "GenAI:API_KEY": "",
    "GenAI:HF_KEY": "",
    "Clasher:API_TOKEN": "",
    "Auth:Secret": "",
    "Auth:Issuer": "",
    "Auth:GoogleClientSecret": "NOT_NEEDED_CURRENTLY",
    "Auth:GoogleClientId": "",
    "Auth:ExpiryTimeFrame": "",
    "Auth:Audience": "",
    "Auth:PgConnection": ""
 ```

5. Build dotnet project by using `dotnet build` and before running angular , make sure to run `npm install` and then `ng build`.

6. If any error occurrs , try checking those .


## 🔗Ref Links :
[Hugging Face](https://huggingface.co/inference-api/serverless)

[Gemini API](https://aistudio.google.com/app/apikey)

[Clash of Clans API](https://aistudio.google.com/app/apikey)

[For making Gmail Mail for Mail sending](https://myaccount.google.com/apppasswords)

<a name="canBeAdded"></a>

## Things can be added :
1. Can modify how we are returning responses , like proper status code by introducing Errors Controller with ProblemDetails object.
2. The UI can be more responsive.
3. Sharable quiz link.
4. Random quiz making.
5. Other external login providers can be added.
6. After quiz result. if user wants to see the question with its correct answer can see.
7. Organizing the exisiting more strictly.