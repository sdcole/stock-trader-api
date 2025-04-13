# stock-trader-api
API for the stock trading application

# Data Flow
Web App <--> App API <--> Rest API <--> DB
# Endpoints
- DataGridController
    - v1/data-grid
        - GET Request that returns the list of available companies to view.
- LineGraphController
    - v1/line-graph
        - GET Request that returns data based off of the view requested of the UI's line graph.
        - Usage: Symbol (company stock symbol to search), Timeframe (The timeframe range to provide data for.)
# Change Log
- 1.0.0 Initial Release
    - Initial release with basic functionality. Interacts between Web app and rest api adding a layer of security.
