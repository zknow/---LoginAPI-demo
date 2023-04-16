# Dotnet Login WebAPI

## _Description_

功能:</br>
    - 登入</br>
    - 登出</br>
    - 取得用戶名稱</br></br>
測試用帳號:</br>
    - Account : admin</br>
    - Password : 0000</br>

## _Tech_

- .NET 6
- EF Core
- jwt
- MySQL
- Redis

## _Docker_

```sh
docker-compose up -d
```

或是debug測試，可以只開db container

```sh
// docker build database container
docker compose -f docker-compose-db.yml up -d

// run debug mode
dotnet run

or

// run release mode
dotnet run --configuration Release  
```

## _API Port_

```sh
5000
```

## _API Doc_

[swagger](https://github.com/zknow/Unionfly-LoginAPI-demo/blob/master/swagger.json)
