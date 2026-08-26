-- SQL migration script to create/update Doctors and Doctor_Availability tables in PostgreSQL / Neon DB

CREATE TABLE IF NOT EXISTS Doctors
(
    Doctor_Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Clinic_Id UUID NULL,
    Full_Name VARCHAR(150) NOT NULL,
    Date_Of_Birth DATE NULL,
    Gender VARCHAR(20) NULL,
    Mobile_Number VARCHAR(20) NULL,
    Address VARCHAR(500) NULL,
    Medical_Registration_Number VARCHAR(50) NULL,
    Registration_State VARCHAR(100) NULL,
    Specialization VARCHAR(100) NULL,
    Sub_Specialization VARCHAR(100) NULL,
    Experience VARCHAR(50) NULL,
    Qualification VARCHAR(100) NULL,
    Consultation_Fees NUMERIC(10,2) NOT NULL DEFAULT 0,
    Available_Days TEXT NULL,
    Is_Active BOOLEAN NOT NULL DEFAULT TRUE,
    Created_On TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Updated_On TIMESTAMP NULL
);

CREATE TABLE IF NOT EXISTS Doctor_Availability
(
    Availability_Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Doctor_Id UUID NOT NULL,
    Session_Type VARCHAR(20) NOT NULL,
    Day_Of_Week VARCHAR(20) NOT NULL,
    From_Time TIME NOT NULL,
    To_Time TIME NOT NULL,
    CONSTRAINT FK_DoctorAvailability_Doctor
        FOREIGN KEY (Doctor_Id)
        REFERENCES Doctors(Doctor_Id)
        ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_Doctors_MedicalRegistrationNumber ON Doctors(Medical_Registration_Number);
CREATE INDEX IF NOT EXISTS IX_Doctor_Availability_Doctor_Id ON Doctor_Availability(Doctor_Id);
