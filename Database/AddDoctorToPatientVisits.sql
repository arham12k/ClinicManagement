ALTER TABLE Patients
    ADD COLUMN IF NOT EXISTS Doctor_Id UUID NULL;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'FK_Patient_Doctor'
    ) THEN
        ALTER TABLE Patients
            ADD CONSTRAINT FK_Patient_Doctor
            FOREIGN KEY (Doctor_Id)
            REFERENCES Doctors(Doctor_Id)
            ON DELETE SET NULL;
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS IX_Patients_Doctor_Id
    ON Patients(Doctor_Id);

ALTER TABLE Patient_Visits
    ADD COLUMN IF NOT EXISTS Doctor_Id UUID NULL;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'FK_PatientVisit_Doctor'
    ) THEN
        ALTER TABLE Patient_Visits
            ADD CONSTRAINT FK_PatientVisit_Doctor
            FOREIGN KEY (Doctor_Id)
            REFERENCES Doctors(Doctor_Id)
            ON DELETE SET NULL;
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS IX_Patient_Visits_Doctor_Id
    ON Patient_Visits(Doctor_Id);