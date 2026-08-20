CREATE TABLE Patients
(
    Patient_Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Clinic_Id UUID NOT NULL,
    Patient_Token VARCHAR(40) NOT NULL UNIQUE,
    Full_Name VARCHAR(150) NOT NULL,
    Age INT NOT NULL,
    Gender VARCHAR(20),
    Mobile_Number VARCHAR(15) NOT NULL,
    Address VARCHAR(500),
    Allergies TEXT,
    Existing_Diseases TEXT,
    Current_Medications TEXT,
    Past_Surgeries TEXT,
    Anc_Profile TEXT,
    Lmp_Date DATE,
    Gestational_Age VARCHAR(50),
    Expected_Delivery_Date DATE,
    Trimester VARCHAR(20),
    Is_Active BOOLEAN NOT NULL DEFAULT TRUE,
    Created_On TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Updated_On TIMESTAMP NULL,
    CONSTRAINT FK_Patient_Clinic
        FOREIGN KEY (Clinic_Id)
        REFERENCES Clinics(Clinic_Id)
        ON DELETE RESTRICT
);

CREATE TABLE Patient_Visits
(
    Patient_Visit_Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Patient_Id UUID NOT NULL,
    Visit_Priority VARCHAR(20) NOT NULL,
    Blood_Pressure VARCHAR(20),
    Sugar_Level VARCHAR(20),
    Weight VARCHAR(20),
    Height VARCHAR(20),
    Temperature VARCHAR(20),
    Pulse_Rate VARCHAR(20),
    Sp_O2 VARCHAR(20),
    Respiratory_Rate VARCHAR(20),
    Created_On TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_PatientVisit_Patient
        FOREIGN KEY (Patient_Id)
        REFERENCES Patients(Patient_Id)
        ON DELETE RESTRICT
);

CREATE INDEX IX_Patients_Clinic_Id ON Patients(Clinic_Id);
CREATE INDEX IX_Patient_Visits_Patient_Id_Created_On ON Patient_Visits(Patient_Id, Created_On DESC);