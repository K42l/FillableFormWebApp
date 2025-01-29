CREATE TABLE departments
(
    department_id     integer primary key autoincrement not null,
    department_name   nvarchar(100) unique not null
);

insert into departments (department_name) values ('Test Department');

CREATE TABLE employees
(
    employee_id         integer primary key autoincrement not null,
    employee_name       nvarchar(250)  not null,
    email               nvarchar(100) not null,
    ssn                 int unique not null,
    department_id       integer not null,
    foreign key(department_id) references departments(department_id)
);

insert into employees (employee_name, email, ssn, department_id) values ('Super Visor', 'supervisor@example.com', 123456789, 1);
insert into employees (employee_name, email, ssn, department_id) values ('John Doe', 'johndoe@example.com', 987654321, 1);

CREATE TABLE supervisors
(
    supervisor_id     integer primary key autoincrement not null,
    department_id     integer not null,    
    employee_id       integer not null,
    foreign key(department_id) references departments(department_id),
    foreign key(employee_id) references employees(employee_id)
);

insert into supervisors (department_id, employee_id) values(1,1);

CREATE TABLE formTypes
(
    formType_id integer primary key autoincrement not null,
    formType_name nvarchar(100) unique not null
);

insert into formTypes (formType_name) values('Leave of Absence');

CREATE TABLE forms
(
    form_id             integer primary key autoincrement not null,
    employee_id         integer not null,
    supervisor_id       integer not null,
    formType_id         integer not null,
    form_date           datetime not null,       
    justitification     nvarchar(450),
    dates               nvarchar(100),
    typeOfLeave         nvarchar(64),
    addtionalRemarks    nvarchar(450),
    decision            nvarchar(50),
    reason              nvarchar(250),
    status              nvarchar(7) not null,
    foreign key(employee_id) references employees(employee_id)
    foreign key(supervisor_id) references supervisors(supervisor_id),
    foreign key(formType_id) references formTypes(formType_id)
);

--select * from aspnetusers
--select * from departments
--select * from employees
--select * from supervisors
--select * from forms
--select * from formTypes
