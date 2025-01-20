# Clinical Trials API

## Prerequisites

- Docker installed on your machine.

## Running the Application

1. Open a terminal and navigate to the ClinicalTrials folder of the application.

2. Run the following command to start the application using Docker Compose:

	docker-compose up -d
	
3. Wait for the SQL Server and Web API containers to start. Once they are running, the API will be available on port `8080`.

## API Endpoints

### Upload Clinical Trial

- **Endpoint**: `POST http://localhost:8080/api/ClinicalTrials/upload`
- **Description**: Uploads a clinical trial information to db.
- **Request Body**: `UploadFileRequest` containing the file to be uploaded.

### Get Clinical Trial by ID

- **Endpoint**: `GET http://localhost:8080/api/ClinicalTrials/trialId`
- **Description**: Retrieves a clinical trial by its ID.
- **Path Parameter**: `trialId` - The ID of the clinical trial.

### Get All Clinical Trials

- **Endpoint**: `GET http://localhost:8080/api/ClinicalTrials/getAll`
- **Description**: Retrieves all clinical trials, optionally filtered by status and title.
- **Query Parameters**:
  - `status` (optional) - The status of the clinical trials to filter by.
  - `title` (optional) - The title of the clinical trials to filter by.
  
## Logs

- To see logs file which is created when api container is run enter following commands:
	DOCKER PS to list containers name
	DOCKER EXEC -IT {{NAME OF API CONTAINER}} /BIN/BASH to enter into api container
	CD COMMON/LOGS navigate to this folder
	LS -LA to check name of logs file 
	CAT {{NAME-OF-LOGS-FILE.TXT}} read the log file
	
## Notes

- Ensure that the Docker containers are running before making any API requests.
- The API will be accessible at `http://localhost:8080`.

