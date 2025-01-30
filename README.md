# Fillable Form Web Application

### A sample application to manage and fill human resources PDF Forms
#### _This is a simplified version of a Razor App that I built for the HR department at my workplace._

# Features

- Register and login with ASP.NET Core Identity for the employees, supervisors and admins to manage their forms.
- Employees can fill new forms and visualize them after the supervisor approves it or not.
- Supervisors have access to the App as an employee and as a supervisor. The supervisor can approve and view the forms assigned to him.
- (In progress) Admins have access to all the forms and they can change the form's status at will.

# Testing

I created three accounts for testing purposes

- Employee account: <br>
  Email - employee@example.com <br>
  Password - employee@123
- Supervisor account: <br>
  Email - supervisor@example.com <br>
  Password - supervisor@123
- Admin account: <br>
  Email - admin@example.com <br>
  Password - admin@123

# Key differences from the original app

- On this application I wanted to use a open source package to deal with the pdf
- In the original one, I used the windows authentication, instead of the Identity, so everyone can access it using their domain account.
- I used SQLite here, instead of sql server, to keep things simple and to make it easy for anyone who wants to download and test it.
- Since I'm using SQLite, I thought it would be best to make use of the Entity Framework. In the original one, I created all the of the sql procedures and used Dapper.
- In this application, there is no option to upload an attachment to the form. Even though adding that option would be easy to do, I'm dynamically generating the forms each time the user clicks on the "eye" icon to visualize it.
- Since there's no Email Sender implemented, it won't actually send the email to the supervisor notifying about a new form. Same thing for ASP.NET Core Identity register, password change and etc.
- The original application has a job scheduled that resends the email to the supervisor after 48 hours of it's creation, if he didn't signed it yet.
- The last (I think) and more significant one. This app has no way of signing and storing the document (Maybe I'll integrate this app with Docusign or something similar, I haven't decided yet).<br> 
The application that I made for the HR department it's integrated with a platform called <a href="https://www.autentique.com.br">Autentique</a>, so the employees can sign their forms after the supervisor approves it. The supervisors sign it automatically with theirs API key, which are stored in the database.<br>
After the employee signs the document, the platform's webhook sends the signed file to an API that download and stores it in our server or a shared Google Drive.

# Packages used

- Entity Framework Core
- ASP.NET Core Identity
- PDFsharp

# Notes

I created a simple database with SQLite to mimic an employee, department and supervisor company's data.<br>
Also, in order to keep it simple, I only used 2 tables to save the information for the pdf forms. Normally I would separate some things, like creating a separate table for the form's status,.

For now it only has one form, a _leave of absence_ pdf that i found at a website with free templates called <a href="https://www.templateroller.com">Template Roller</a>. But it's fairly simple to add new types of form.

In order to turn that pdf into a fillable one, I used the <a href="https://www.docfly.com">DocFly</a> website, which is free to use. The only limitation for the free usage is the amount of times that you can download the files from there, 3 times a month if I'm not mistaken, which shouldn't be a problem unless you intend to create a bunch of fillable forms. 

I tried to keep the JavaScript at a minimum, to make the most use of Razor's functionality. I used JQuery but it can easily be implemented with plain JavaScript.

I stored the pdf's fields name and maxLenght in a json, to make it easy to match the pdf's fields to the form.

Since the SQLite tables are just for demonstration purposes, there's no way of adding or managing the employees, supervisors, departments and form types tables.

Fields with multiple lines in the pdf, like the purpose for leave field, are named with a "0x" at the and to make it easier to find the lines that belong to that field. EX: purposeForLeave01, purposeForLeave02 and purposeForLeave03