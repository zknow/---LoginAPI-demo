# Dotnet Login WebAPI

## _Description_

Using : .net core6 WebAPI + EF + MySQL + Redis</br>
API:</br>
    - Login</br>
    - UserName</br>
    - Logout</br>

## _Tech_

- [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [EF Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)
- [jwt](https://learn.microsoft.com/zh-tw/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer?view=aspnetcore-7.0)

## _Docker_

```sh
docker-compose up -d
```

debug mode測試，可以只run db container

```sh
// docker build database container
docker compose -f docker-compose-db.yml up -d

// run debug mode
dotnet run

// run release mode
dotnet run --configuration Release  
```

## _API Port_

```sh
5000:5000
```

## _API Doc_

[swagger]()
