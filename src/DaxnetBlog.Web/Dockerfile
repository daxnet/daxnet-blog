FROM microsoft/dotnet:latest
RUN apt-get install -y libgdiplus
RUN cd /usr/local/src
RUN mkdir app
WORKDIR /usr/local/src/app
 
COPY . ./
 
EXPOSE 5000

CMD ["dotnet", "DaxnetBlog.Web.dll"]
