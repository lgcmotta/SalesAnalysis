# Sales Analysis

A microservices architecture demonstration using Docker, RabbitMQ and .NET Core 3.0.

## Application purpose

FileWatcher will look for new files added to C:\HOMEPATH\data\in (or var/lib/HOMEPATH/data/in for Linux).

SalesProcessor will read the file and process some fake data. The application waits for a txt or a csv file using 'ç' as saparator.

FileGenerator will write an output file in 
C:\HOMEPATH\data\out (or var/lib/HOMEPATH/data/out for Linux)

## Environment

- .NET Core 3.0 preview 6
- Docker for Desktop

## Samples

### Input Sample

```txt
001ç1234567891234çPedroç50000
001ç3245678865434çPauloç40000.99
002ç2345675434544345çJose da SilvaçRural
002ç2345675433444345çEduardo PereiraçRural
003ç10ç[1-10-100,2-30-2.50,3-40-3.10]çPedro
003ç08ç[1-34-10,2-33-1.50,3-40-0.10]çPaulo
```

### Output Sample

```txt
--------- Original Input File: SalesSample001.txt

--------- Salesmen Count: 2
--------- Customers Count: 2
--------- Most Expensive Sale Id: 10
--------- Worst Salesman: Paulo

--------- Output Generated At: 2019-08-22 19:44:38
```
