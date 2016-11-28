FROM microsoft/dotnet:1.1.0-runtime
RUN apt-get update && apt-get install -y libgdiplus
RUN cd /usr/local/src
RUN mkdir app
WORKDIR /usr/local/src/app
 
COPY . ./

EXPOSE 5000
EXPOSE 5001

CMD ["dotnet", "DaxnetBlog.Web.dll"]
