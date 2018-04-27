# Open Audience

Open Audience uses open data to analyse the demographics of your audience or users. Paste in a list of postcodes and see what type of people live there.

Try it at [openaudience.org](openaudience.org).

## Method and data sources
We take the following data,
* [Pen Portraits, from the ONS](https://www.ons.gov.uk/methodology/geography/geographicalproducts/areaclassifications/2011areaclassifications/datasets)
* [Approximated Social Grade, from Nomis](https://www.nomisweb.co.uk/census/2011/qs611ew)
* [Age by single year, from Nomis Web](https://www.nomisweb.co.uk/census/2011/qs103ew)
* [Small area model-based income estimates, England and Wales, from the ONS](https://www.ons.gov.uk/peoplepopulationandcommunity/personalandhouseholdfinances/incomeandwealth/bulletins/smallareamodelbasedincomeestimates/financialyearending2014)
* [Ethnic group, by Nomis Web](https://www.nomisweb.co.uk/census/2011/qs201ew)
* [Postcode to Output Area to ... from the ONS](https://ons.maps.arcgis.com/home/item.html?id=ef72efd6adf64b11a2228f7b3e95deea)
* [Output Area to ... to MSOA from the ONS](http://geoportal.statistics.gov.uk/datasets/output-area-to-local-authority-district-to-lower-layer-super-output-area-to-middle-layer-super-output-area-to-local-enterprise-partnership-april-2017-lookup-in-england-v2)

We process it in C# to create two tables,

1. Postcode to Output Area lookup.
2. Output Area to data lookup (there are over 30 columns in this table).

These are then served from a database, via PHP, to an HTML5 app that process user input and displays the results.

## License
The code for processing the raw data is licensed under the MIT license. Data is licensed under the [Open Government License v3 (OGL)](http://www.nationalarchives.gov.uk/doc/open-government-licence/version/3/), as it based on OGL licensed content itself.

## Funding
This work was part-funded and supported by [Leeds Capital of Culture 2023](http://leeds2023.co.uk/) and Leeds City Council. It has been tested in Leeds and Bradford with Universities, small arts venues, and at The Open Data Institute Leeds.
