FROM microsoft/dotnet:latest
RUN cd /usr/local/src
RUN mkdir app
WORKDIR /usr/local/src/app
 
COPY . ./
 
EXPOSE 5000

# 执行dotnet DockerWebAPI.dll命令
CMD ["dotnet", "DaxnetBlog.WebServices.dll"]
