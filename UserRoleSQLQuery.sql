CREATE TABLE UserAccouuntsRoles
(
ID int Identity,
Name nvarchar(50),
Username varchar(20) Unique,
AccessLevel int,
PRIMARY KEY (ID)
);

CREATE TABLE UserAccouunts
(
ID int Identity,
Name nvarchar(50),
Code varchar(20) Unique,
Passkey varchar(max),
CreateDate DateTime,
LastTimeOnline DateTime,
RoleID int,
ProfileImage varbinary(max),
Phone varchar(15),
Email varchar(81),
Approved bit,
PRIMARY KEY (ID)
);

ALTER TABLE UserAccouunts 
ADD CONSTRAINT FK_UARole FOREIGN KEY (RoleID) REFERENCES UserAccouuntsRoles (ID);

INSERT INTO UserAccouuntsRoles VALUES
( 'مدیر سایت', 'admin', 1 ), ( 'منشی', 'secretary', 2 ), ( 'مسئول فروش', 'sale', 3 ), ( 'مسئول اجاره', 'rent', 4 ), ( 'کاربر عادی', 'regulat', 5 )

DELETE FROM [dbo].[Features-jnc]
WHERE [StateId] = 9120

ALTER TABLE [Features-jnc]
ALTER COLUMN StateId int Not NULL

ALTER TABLE [Features-jnc]
ALTER COLUMN ItemId int Not NULL

ALTER TABLE [Features-jnc]
ADD CONSTRAINT pk_Features_JNS_ID Primary Key(StateId, ItemId)

ALTER TABLE [dbo].[FreeAdvertise]
ADD PRIMARY KEY (ID)

ALTER TABLE dbo.FreeAdvertise
ADD edit_key VARCHAR(35) NULL;

ALTER TABLE dbo.FreeAdvertise ALTER COLUMN title NVARCHAR (50) NOT NULL;

CREATE TABLE AdverKeywords
(
ID int Identity,
Name nvarchar(40),
PRIMARY KEY (ID)
);

ALTER TABLE AdverKeywords
ADD Wieght int;

create table adver_keyword_junction
(
  adver_id int,
  keyword_id int,
  CONSTRAINT adver_keyw_pk PRIMARY KEY (adver_id, keyword_id),
  CONSTRAINT FK_adver 
      FOREIGN KEY (adver_id) REFERENCES FreeAdvertise (ID),
  CONSTRAINT FK_keyword 
      FOREIGN KEY (keyword_id) REFERENCES AdverKeywords (ID)
);

INSERT INTO AdverKeywords (Name, Wieght)
VALUES 
('نوساز', 9),
('کلید نخورده', 9),
('بازسازی شده', 9),
('تخلیه', 9),
('مجردی', 8),
('مناسب زوج', 8),
('خانوادگی', 8),
('سوئیت', 7),
('به قیمت رسیده', 7),
('اجاره قابل تبدیل', 7),
('لوکس', 6),
('فرعی دنج', 6),
('دسترسی مناسب', 7),
('طبقات بالا', 5),
('طبقات پایین', 5),
('آسانسور', 8),
('انباری', 5),
('پارکینگ', 8),
('متریال مقاوم', 4),
('نورگیر', 7),
('شمالی', 4),
('سرویس ایرانی', 4),
('سرویس فرنگی', 4),
('گاز رومیزی', 4),
('آنتن مرکزی', 2),
('MDF', 2)

