# Many In One

It is a fully functional web application written in .NET and Angular frameworks where we can use secure login/registeration with two factor authentication, some useful generative AI functionalities, quiz maker with ability to play etc. 

[Live URL](https://manyinone-gq6db2ujvq-ue.a.run.app)

[GitHub](https://github.com/anikdakua03/Many-In-One)

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

![Home](https://github.com/anikdakua03/Many-In-One/assets/83277115/cf3d5889-0e88-4350-ab4e-060366b94671)


## Tech Stack :

**Client :** Angular 17, TailwindCSS

**Server :** ASP.NET Core Web API [.NET 8]

**Database :** MSSQL

## Available features with some screenshots :

1. Secure Login / Registration with two factor authentication .
    - Login
      ![Login](https://github.com/anikdakua03/Many-In-One/assets/83277115/dbfb6751-0b5a-479e-8eb7-a4c5c8c6a9b1)

    - Registration
      ![Sign Up](https://github.com/anikdakua03/Many-In-One/assets/83277115/641007b9-e99b-4ca3-ac38-1f138f0371c7)

    
2. User profile and other details management
    - Enable two factor
      ![Enable 2fa](https://github.com/anikdakua03/Many-In-One/assets/83277115/02972a85-12e6-4b99-ab36-1a482f695772)
      
    - Disable two factor
      ![Disable 2fa](https://github.com/anikdakua03/Many-In-One/assets/83277115/153ef762-ec7a-4d96-9a46-e0b0f06dfafc)

    - Delete all user data
      ![20  delete all](https://github.com/anikdakua03/Many-In-One/assets/83277115/4a707976-7864-4dfa-a210-c96caef12baf)

    - Quiz management
       ![Category list](https://github.com/anikdakua03/Many-In-One/assets/83277115/7b94e9d4-42de-4aeb-96cc-c1637e19de6e)

        - Add / update / delete category
           ![Category add](https://github.com/anikdakua03/Many-In-One/assets/83277115/0f72e131-a49d-4780-a100-bab8318aab5a)

        - Add / update / delete question
          ![Add-edit](https://github.com/anikdakua03/Many-In-One/assets/83277115/204ee3b4-f7d4-41a1-a293-483026c6d9fd)

        - Add multiple questions after filling the provided question template and upload
          ![Qs list](https://github.com/anikdakua03/Many-In-One/assets/83277115/84e7c819-c42c-43ec-ac3d-6ac33dadd11c)

4. CRUD operation 
    - Like simple payment card addition / deleteion / updation
      ![Payment CRUD](https://github.com/anikdakua03/Many-In-One/assets/83277115/420eb72c-2674-499b-aae8-39fc1c13e006)


5. Use some generative AI tools
   ![Gen AI options](https://github.com/anikdakua03/Many-In-One/assets/83277115/6c04123d-6f2e-49d5-ab8c-17bcbdd70507)

    - Provide your query and get answer to it
    - Provide image and its related question and get answer
    - Summarize long text
    - Describe your thoughts in a text to generate image
    - Provide some text to convert to audio
    - Conversation like qna with gen AI
      
      ![Gen AI text](https://github.com/anikdakua03/Many-In-One/assets/83277115/778c1892-7926-4c6c-a9aa-73d278ee0bb1)


7. Quiz maker and play quiz 
    - Can create quiz of any available categories and play that.
      ![Quiz creation](https://github.com/anikdakua03/Many-In-One/assets/83277115/63b7cefd-50bb-464d-b466-3e1a8a396cde)

    - Otherwise first create your own set of quiz questions and then play with it.
      ![Quiz score](https://github.com/anikdakua03/Many-In-One/assets/83277115/3e76401a-3a44-428e-a4e5-bc0f6aa71e8c)

8. Check some clash of clan details
   ![COC options](https://github.com/anikdakua03/Many-In-One/assets/83277115/9ec79672-4b43-443d-8935-523a5488cf01)

    - Search player and see details.
      ![Player](https://github.com/anikdakua03/Many-In-One/assets/83277115/9e4c4661-4acc-4f2b-b476-7e6ea024b1fc)

    - Search clan by tag or search clans with some filters 
      ![Clan search](https://github.com/anikdakua03/Many-In-One/assets/83277115/a1c61462-bf4c-45bf-8b0a-b02da3afb1b7)
      
      ![Clans](https://github.com/anikdakua03/Many-In-One/assets/83277115/a445ac6d-8a4c-4e66-a323-3acaefc2b471)


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
> > Project [live URL](https://manyinone-gq6db2ujvq-ue.a.run.app) may not work due to expiration of billing.

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

5. Also apply database migration --
   ```cs
   dotnet ef migrations add "MIGRATION_NAME"
   // then update databse
   dotnet ef database update
   ```
7. Now try building dotnet project by using `dotnet build` and before running angular , make sure to run `npm install` and then `ng build`.

8. If any error occurrs , try checking the console .


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
