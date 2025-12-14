
# Enable Logging in Azure App Service

## Application Logging

### Enable Application Logging
```bash
# Enable application logging (filesystem)
az webapp log config --name <app-name> --resource-group <resource-group> --application-logging filesystem

# Enable application logging (blob storage)
az webapp log config --name <app-name> --resource-group <resource-group> --application-logging azureblobstorage
```

### Configure Log Levels
- **Error**: Only error messages
- **Warning**: Error and warning messages
- **Information**: Error, warning, and informational messages
- **Verbose**: All messages

## Web Server Logging

### Enable Web Server Logs
```bash
# Enable web server logging
az webapp log config --name <app-name> --resource-group <resource-group> --web-server-logging filesystem
```

## Detailed Error Messages

```bash
# Enable detailed error messages
az webapp log config --name <app-name> --resource-group <resource-group> --detailed-error-messages true
```

## Failed Request Tracing

```bash
# Enable failed request tracing
az webapp log config --name <app-name> --resource-group <resource-group> --failed-request-tracing true
```

## View Logs

```bash
# Stream logs in real-time
az webapp log tail --name <app-name> --resource-group <resource-group>

# Download logs
az webapp log download --name <app-name> --resource-group <resource-group>
```

## Log Locations

- **Application logs**: `/LogFiles/Application/`
- **Web server logs**: `/LogFiles/http/RawLogs/`
- **Detailed error logs**: `/LogFiles/DetailedErrors/`
- **Failed request logs**: `/LogFiles/W3SVC#########/`
