FROM microsoft/dotnet:latest
RUN cd /usr/local/src
RUN mkdir app
WORKDIR /usr/local/src/app
 
COPY . ./
 
CMD ["dotnet", "DaxnetBlog.DbMigration.dll"]
