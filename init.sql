CREATE TABLE "EducationTypes" (
    "Id" SERIAL PRIMARY KEY,
    "EducationType" VARCHAR
);

CREATE TABLE "LessonTypes" (
    "Id" SERIAL PRIMARY KEY,
    "Type" VARCHAR
);

CREATE TABLE "Posts" (
    "Id" SERIAL PRIMARY KEY,
    "Post" VARCHAR
);

CREATE TABLE "Specialities" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR,
    "Description" VARCHAR
);

CREATE TABLE "Grades" (
    "Id" SERIAL PRIMARY KEY,
    "Grade" VARCHAR
);

CREATE TABLE "StudyDurations" (
    "Id" SERIAL PRIMARY KEY,
    "Period" VARCHAR,
    "EducationTypeId" INTEGER REFERENCES "EducationTypes"("Id")
);

CREATE TABLE "StudyDurationSpeciality" (
    "Id" SERIAL PRIMARY KEY,
    "StudyDurationId" INTEGER REFERENCES "StudyDurations"("Id"),
    SpecialityId INTEGER REFERENCES "Specialities"("Id")
);

CREATE TABLE "Subjects" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR,
    "Description" VARCHAR
);

CREATE TABLE "Employees" (
    "Id" SERIAL PRIMARY KEY,
    "Surname" VARCHAR,
    "Name" VARCHAR,
    "Patronymic" VARCHAR,
    "Birthday" DATE,
    "Login" VARCHAR,
    "Password" VARCHAR,
    "Phone" VARCHAR,
    "Salary" NUMERIC,
    "PostId" INTEGER REFERENCES "Posts"("Id")
);

CREATE TABLE "SubjectSpecialty" (
    "Id" SERIAL PRIMARY KEY,
    "SubjectId" INTEGER REFERENCES "Subjects"("Id"),
    SpecialityId INTEGER REFERENCES "Specialities"("Id")
);

CREATE TABLE "SubjectEmployee" (
    "Id" SERIAL PRIMARY KEY,
    "SubjectId" INTEGER REFERENCES "Subjects"("Id"),
    "PossibleEmployeeId" INTEGER REFERENCES "Employees"("Id")
);

CREATE TABLE "Groups" (
    "Id" SERIAL PRIMARY KEY,
    "CuratorId" INTEGER REFERENCES "Employees"("Id"),
    "Code" VARCHAR,
    "Course" INTEGER,
    "SpecialityId" INTEGER REFERENCES "Specialities"("Id")
);

CREATE TABLE "Students" (
    "Id" SERIAL PRIMARY KEY,
    "Surname" VARCHAR,
    "Name" VARCHAR,
    "Patronymic" VARCHAR,
    "Birthday" DATE,
    "Login" VARCHAR,
    "Password" VARCHAR,
    "GroupId" INTEGER REFERENCES "Groups"("Id"),
    "Phone" VARCHAR
);

CREATE TABLE "Lessons" (
    "Id" SERIAL PRIMARY KEY,
    "SubjectId" INTEGER REFERENCES "Subjects"("Id"),
    "TeacherId" INTEGER REFERENCES "Employees"("Id"),
    "Date" TIMESTAMP WITHOUT TIME ZONE,
    "GroupId" INTEGER REFERENCES "Groups"("Id"),
    "Number" INTEGER,
    "LessonTypeId" INTEGER REFERENCES "LessonTypes"("Id")
);

CREATE TABLE "StudentGrades "(
    "Id" SERIAL PRIMARY KEY,
    "StudentId" INTEGER REFERENCES "Students"("Id"),
    "LessonId" INTEGER REFERENCES "Lessons"("Id"),
    "GradeId" INTEGER REFERENCES "Grades"("Id"),
    "Date" TIMESTAMP WITHOUT TIME ZONE,
    "Description" VARCHAR
);