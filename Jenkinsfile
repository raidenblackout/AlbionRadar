// Jenkinsfile with post-build workspace cleanup

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
        
        stage('Restore Packages with nuget.exe') {
            steps {
                echo "Restoring NuGet packages for the entire solution..."
                bat 'nuget restore AlbionRadar.sln'
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
    
    // This section runs after all the stages are complete.
    post {
        always {
            echo 'Build process finished. Now cleaning up the workspace...'
            
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