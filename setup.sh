#!/bin/bashInstall
#Project backend
cd backend

#Install nuget packages
echo "Installing necesary nuget packages for backend"

dotnet add package Swashbuckle.AspNetCore --version 6.4.0
dotnet add package Microsoft.AspNetCore.OpenApi --version 7.0.2
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 7.0.15
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 7.0.15
dotnet add package Microsoft.EntityFrameworkCore.Design --version 7.0.15



# Modify the string conection with your own database server.
content="Server={YourServerInstance};Database=NotesWebApp;User Id={YourUser};Password={YourPassword};Encrypt=False;"

# Do not modify this file name
file_name="StringConnection.txt"

# Write content to the file
echo "$content" > "$file_name"

echo "File '$file_name' has been created with the following content:"
cat "$file_name"

#Adding migration
echo "Adding migration"
add-migration FirstMigration
update-database


cd ..
cd frontend
echo "Installing necesary nuget packages for frontend"
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 7.0.11
dotnet add package Newtonsoft.Json --version 13.0.3

# Modify the with the name principal URL of your web api.
content2="http://localhost/"

# Do not modify this file name
file_name2="Url.txt"

# Write content to the file
echo "$content2" > "$file_name2"

echo "File '$file_name2' has been created with the following content:"
cat "$file_name2"

echo "Script completed."