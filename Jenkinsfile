// Jenkinsfile for building directly on the Windows agent (no Docker)

pipeline {
    agent { label 'windows' }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_NOLOGO = '1'
    }

    stages {
        stage('Clean Workspace') {
            steps {
                echo 'Ensuring a clean build environment...'
                deleteDir()
            }
        }

        stage('Checkout Code') {
            steps {
                echo "Checking out source code from Git..."
                checkout scm
            }
        }

        stage('Clear NuGet Cache') {
            steps {
                echo "Clearing all local NuGet caches on the agent..."
                bat 'dotnet nuget locals all --clear'
            }
        }
        
        stage('Restore NuGet Packages') {
            steps {
                echo "Restoring NuGet packages for AlbionRadar.sln..."
                
                bat 'dotnet restore AlbionRadar.sln -v n'
            }
        }

        stage('Build Solution') {
            steps {
                echo "Building entire solution in Release mode..."
                bat 'dotnet build AlbionRadar.sln -c Release --no-restore'
            }
        }

        stage('Publish WPF Application') {
            steps {
                echo "Publishing the main WPF application to the 'output' directory..."
                bat 'dotnet publish AlbionRadar/AlbionRadar.csproj -c Release -o output --no-build'
            }
        }

        stage('Archive Artifacts') {
            steps {
                echo "Archiving the contents of the 'output' directory..."
                archiveArtifacts artifacts: 'output/**/*', allowEmptyArchive: false
            }
        }
    }
    
    post {
        always {
            echo 'Build process finished.'
            cleanWs()
        }
        success {
            echo 'Build was successful!'
        }
        failure {
            echo 'BUILD FAILED. Check the logs for details.'
        }
    }
}