dotnet clean
start "Currycomb.Gateway"     /D Currycomb.Gateway     "dotnet" watch run
start "Currycomb.AuthService" /D Currycomb.AuthService "dotnet" watch run
start "Currycomb.ChatService" /D Currycomb.ChatService "dotnet" watch run
start "Currycomb.PlayService" /D Currycomb.PlayService "dotnet" watch run
