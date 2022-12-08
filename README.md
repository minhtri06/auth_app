# JwtApplication

## About the project

This project implement the basic authentication and authorization for api using asp.net api

-   Hash user password with salt
-   Token base authorization (JWT with access and refresh token)
-   Role based authorization

This app implement 4 api endpoint:

-   Register new user: Create new user and get token
-   Login: Get token
-   Refresh token: Refresh token
-   Get all user (For testing)

An access token will expire in 30 seconds. A refresh token will expire in 60 seconds.

## Built with

-   .NET v6.0.403 asp.net api
-   AspNetCore.Authentication.JwtBearer v6.0.6 (For authentication and authorization)
-   EntityFrameworkCore v7.0.0
-   EntityFrameworkCore.SqlServer v7.0.0
-   EntityFrameworkCore.Tools v7.0.0

## Contact

You can contact with me by

-   Email: pmtri.tvv@gmail.com
-   Facebook: https://facebook.com/pmtritvv/
-   Linkedin: https://linkedin.com/in/minh-tr%C3%AD-ph%E1%BA%A1m-b4a646257/
