CREATE TABLE Car (
  CarID int PRIMARY KEY IDENTITY(1,1),
  Model varchar(255),
  Make varchar(255),
  LicensePlate varchar(255),
  DayRate money
);

CREATE TABLE RentalContract (
  RentalContractID int PRIMARY KEY IDENTITY(1,1),
  CustomerID int,
  CarID int,
  StartRental datetime,
  EndRental datetime,
  FinalSettlement decimal
);

CREATE TABLE Customer (
  CustomerID int PRIMARY KEY IDENTITY(1,1),
  Name varchar(255),
  FirstName varchar(255),
  Email varchar(255),
  Phone varchar(255),
  BirthDate datetime
);

ALTER TABLE "RentalContract" ADD FOREIGN KEY ("CustomerID") REFERENCES "Customer" ("CustomerID");

ALTER TABLE "RentalContract" ADD FOREIGN KEY ("CarID") REFERENCES "Car" ("CarID");