# OAHub

An office automation service on web for small teams

---

#### Project Status (GitHub)
![License](https://img.shields.io/github/license/Ranzeplay/OAHub)
![Language](https://img.shields.io/github/languages/top/Ranzeplay/OAHub)
![GitHub commit activity](https://img.shields.io/github/commit-activity/w/Ranzeplay/OAHub)
![GitHub last commit](https://img.shields.io/github/last-commit/Ranzeplay/OAHub)
![GitHub repo size](https://img.shields.io/github/repo-size/Ranzeplay/OAHub)
![GitHub issues](https://img.shields.io/github/issues/Ranzeplay/OAHub)
![GitHub tag (latest by date)](https://img.shields.io/github/v/tag/Ranzeplay/OAHub)

#### Build Status (Azure DevOps Pipeline)

- master: [![Build Status](https://dev.azure.com/ranzeplay/OAHub/_apis/build/status/%5BRelease%20Master%20Branch%5D%20%20OAHub-ASP.NET%20Core-CI?branchName=master)](https://dev.azure.com/ranzeplay/OAHub/_build/latest?definitionId=6&branchName=master)

- Other branches: [![Build Status](https://dev.azure.com/ranzeplay/OAHub/_apis/build/status/Ranzeplay.OAHub?branchName=master)](https://dev.azure.com/ranzeplay/OAHub/_build/latest?definitionId=5&branchName=master)
  

## Available Services

- **Passport**
  - Integrated OAuth authorization and User authentication using ASP.NET Core Identity.
- **Storage**
  - An online storage service, but not recommended to store personal files.
- **Status**
  - A monitor system to let everyone know the status of an *App*
- **Organization**
  - A member management service.
  - *Extension-As-Body*.
- **Workflow**
  - An extension for Organization.
  - A working progress tracker.



## Database

We are using **MySQL** currently. But you can also use other databases(SQLite, SQLServer, etc.) by editing the code.

Because **SQLServer** is too expensive, I can only use *Express Edition* :)



