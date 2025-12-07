CREATE TABLE "EducationTypes" (
    "Id" SERIAL PRIMARY KEY NOT NULL,
    "EducationType" VARCHAR(50) NOT NULL
);

CREATE TABLE "LessonTypes" (
    "Id" SERIAL PRIMARY KEY NOT NULL,
    "Type" VARCHAR(30) NOT NULL
);

CREATE TABLE "Posts" (
    "Id" SERIAL PRIMARY KEY NOT NULL,
    "Post" VARCHAR(50) NOT NULL
);

CREATE TABLE "Specialities" (
    "Id" SERIAL PRIMARY KEY NOT NULL,
    "Name" VARCHAR(100) NOT NULL,
    "Description" VARCHAR(255)
);

CREATE TABLE "Grades" (
    "Id" SERIAL PRIMARY KEY NOT NULL,
    "Grade" VARCHAR(10) NOT NULL
);

CREATE TABLE "StudyDurations" (
    "Id" SERIAL PRIMARY KEY NOT NULL,
    "Period" VARCHAR(50) NOT NULL,
    "EducationTypeId" INTEGER REFERENCES "EducationTypes"("Id") NOT NULL
);

CREATE TABLE "StudyDurationSpeciality" (
    "Id" SERIAL PRIMARY KEY NOT NULL,
    "StudyDurationId" INTEGER REFERENCES "StudyDurations"("Id") NOT NULL,
    "SpecialityId" INTEGER REFERENCES "Specialities"("Id") NOT NULL
);

CREATE TABLE "Subjects" (
    "Id" SERIAL PRIMARY KEY NOT NULL,
    "Name" VARCHAR(100) NOT NULL,
    "Description" VARCHAR(255)
);

CREATE TABLE "Employees" (
    "Id" SERIAL PRIMARY KEY NOT NULL,
    "Surname" VARCHAR(50) NOT NULL,
    "Name" VARCHAR(50) NOT NULL,
    "Patronymic" VARCHAR(50),
    "Birthday" DATE NOT NULL,
    "Login" VARCHAR(50) NOT NULL UNIQUE,
    "Password" VARCHAR(256) NOT NULL,
    "Phone" VARCHAR(20) UNIQUE,
    "Salary" NUMERIC NOT NULL,
    "PostId" INTEGER REFERENCES "Posts"("Id") NOT NULL
);

CREATE TABLE "SubjectSpecialty" (
    "Id" SERIAL PRIMARY KEY NOT NULL,
    "SubjectId" INTEGER REFERENCES "Subjects"("Id") NOT NULL,
    "SpecialityId" INTEGER REFERENCES "Specialities"("Id") NOT NULL
);

CREATE TABLE "SubjectEmployee" (
    "Id" SERIAL PRIMARY KEY NOT NULL,
    "SubjectId" INTEGER REFERENCES "Subjects"("Id") NOT NULL,
    "PossibleEmployeeId" INTEGER REFERENCES "Employees"("Id") NOT NULL
);

CREATE TABLE "Groups" (
    "Id" SERIAL PRIMARY KEY NOT NULL,
    "CuratorId" INTEGER REFERENCES "Employees"("Id") NOT NULL,
    "Code" VARCHAR(20) NOT NULL,
    "Course" INTEGER NOT NULL,
    "SpecialityId" INTEGER REFERENCES "Specialities"("Id") NOT NULL
);

CREATE TABLE "Students" (
    "Id" SERIAL PRIMARY KEY NOT NULL,
    "Surname" VARCHAR(50) NOT NULL,
    "Name" VARCHAR(50) NOT NULL,
    "Patronymic" VARCHAR(50),
    "Birthday" DATE NOT NULL,
    "Login" VARCHAR(50) UNIQUE,
    "Password" VARCHAR(256),
    "GroupId" INTEGER REFERENCES "Groups"("Id") NOT NULL,
    "Phone" VARCHAR(20) UNIQUE
);

CREATE TABLE "Lessons" (
    "Id" SERIAL PRIMARY KEY NOT NULL,
    "SubjectId" INTEGER REFERENCES "Subjects"("Id") NOT NULL,
    "TeacherId" INTEGER REFERENCES "Employees"("Id") NOT NULL,
    "Date" TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    "GroupId" INTEGER REFERENCES "Groups"("Id") NOT NULL,
    "Number" INTEGER NOT NULL,
    "LessonTypeId" INTEGER REFERENCES "LessonTypes"("Id") NOT NULL
);

CREATE TABLE "StudentGrades" (
    "Id" SERIAL PRIMARY KEY NOT NULL,
    "StudentId" INTEGER REFERENCES "Students"("Id") NOT NULL,
    "LessonId" INTEGER REFERENCES "Lessons"("Id") NOT NULL,
    "GradeId" INTEGER REFERENCES "Grades"("Id") NOT NULL,
    "Date" TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    "Description" VARCHAR(255)  
);