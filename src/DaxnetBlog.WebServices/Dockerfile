FROM microsoft/dotnet:1.0.1-runtime
RUN cd /usr/local/src
RUN mkdir app
WORKDIR /usr/local/src/app
 
COPY . ./
 
EXPOSE 5000

CMD ["dotnet", "DaxnetBlog.WebServices.dll"]
