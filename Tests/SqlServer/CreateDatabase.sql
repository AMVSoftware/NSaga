Use master
Go

if (select DB_ID('NSaga-Testing')) is not null
begin
	alter database [NSaga-Testing] set offline with rollback immediate;
	alter database [NSaga-Testing] set online;
	drop database [NSaga-Testing]
end

create database [NSaga-Testing]

use [NSaga-Testing]
Go