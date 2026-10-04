create database visitordb;
use visitordb;

CREATE TABLE Visitors (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    VisitedAtUtc DATETIME NOT NULL
);