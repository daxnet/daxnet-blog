FROM microsoft/dotnet:1.0.1-runtime
RUN apt-get update && apt-get install -y libgdiplus
RUN cd /usr/local/src
RUN mkdir app
WORKDIR /usr/local/src/app
 
COPY . ./
 
EXPOSE 5000

CMD ["dotnet", "DaxnetBlog.Web.dll"]
