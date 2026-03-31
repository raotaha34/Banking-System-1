
print('______________________________')
print('Database Of the Banking System')
print('______________________________')

use bs
select * from AspNetUsers
select * from Accounts
select * from Transactions

DBCC CHECKIDENT('Transactions', NORESEED);

-- update Transactions set TransactionType = 'Transfer' where TransactionId = 17

--update Accounts set InterestRate = 1 where AccountId = 12


-- update Transactions set ReceiverName = 'Wali Ahmed' where TransactionId = 7
-- update Transactions set ReceiverName = 'Rao Taha' where TransactionId = 4

-- select * from sys.tables


-- UPDATE AspNetUsers SET CreatedAt = GETDATE() WHERE CreatedAt = '0001-01-01';
-- UPDATE Accounts SET CreatedAt = GETDATE() WHERE CreatedAt = '0001-01-01';



--select * from AspNetUserRoles
--Select * from AspNetRoles


--update AspNetUsers set FullName='RaoTahaAli' where Id ='90a2817a-98ea-4c2e-b3e7-6fd1facf3e85'
--update AspNetUserRoles set RoleId = '60b24d28-419a-4f83-bddc-2b9c7d1b108b' where UserId = 'db11a2fe-45f0-43b7-95ee-57f47f85e00e'



--SELECT * FROM AspNetRoles 
--WHERE Id = '60b24d28-419a-4f83-bddc-2b9c7d1b108b';



/*

INSERT INTO Accounts
(
 
    UserId,
    AccountNumber,
    AccountType,
    Balance,
    InterestRate,
    OpeningDate,
    Status,
    DailyLimit,
    MonthlyLimit,
    Notes
)
VALUES
(
                           -- AccountId (use auto-increment if your DB handles it)
    '90a2817a-98ea-4c2e-b3e7-6fd1facf3e85',          -- UserId (the ID of the user in ApplicationUser table)
    '12345678845590',             -- AccountNumber
    'Savings',                -- AccountType
    3470000,                  -- Balance
    0,                     -- InterestRate (nullable if not needed)
    '2026-03-17 13:10:50.7159813',             -- OpeningDate (YYYY-MM-DD)
    'Active',                 -- Status
    null,                     -- DailyLimit (nullable if not needed)
    null,                   -- MonthlyLimit (nullable if not needed)
    null -- Notes (nullable)
)*/



--select * from AspNetRoleClaims
--select * from AspNetUserClaims
--select * from AspNetUserTokens
--select * from AspNetRoleClaims

