FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine3.18 as root

WORKDIR /sln

COPY ./SimpleDbUp.sln ./SimpleDbUp.sln
COPY ./SimpleDbUp/SimpleDbUp.csproj ./SimpleDbUp/SimpleDbUp.csproj

RUN dotnet restore

COPY . .

RUN cd SimpleDbUp && \
   dotnet publish \
      -c Release \
    #   --no-build \
    # --no-self-contained \
      -o /publish

FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine3.18 as release

COPY --from=root /publish /app

ENTRYPOINT [ "/app/SimpleDbUp" ]
