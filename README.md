# Open Audience

Open Audience uses open data to analyse the demographics of your audience or users. Paste in a list of postcodes and see what type of people live there.

Try it at [openaudience.org](openaudience.org).

## Method and data sources.
We take the following data,
* [Pen Portraits, from the ONS](https://www.ons.gov.uk/methodology/geography/geographicalproducts/areaclassifications/2011areaclassifications/datasets)
* [Approximated Social Grade, from Nomis](https://www.nomisweb.co.uk/census/2011/qs611ew)
* [Age by single year, from Nomis Web](https://www.nomisweb.co.uk/census/2011/qs103ew)
* [Highest qualification, from Nomis Web](https://www.nomisweb.co.uk/census/2011/qs501ew)
* [Small area model-based income estimates, England and Wales, from the ONS](https://www.ons.gov.uk/peoplepopulationandcommunity/personalandhouseholdfinances/incomeandwealth/bulletins/smallareamodelbasedincomeestimates/financialyearending2014) -- we use the after housing costs estimate.
* [Ethnic group, by Nomis Web](https://www.nomisweb.co.uk/census/2011/qs201ew)
* [Postcode to Output Area to ... from the ONS](https://ons.maps.arcgis.com/home/item.html?id=ef72efd6adf64b11a2228f7b3e95deea)
* [Output Area to ... to MSOA from the ONS](http://geoportal.statistics.gov.uk/datasets/output-area-to-local-authority-district-to-lower-layer-super-output-area-to-middle-layer-super-output-area-to-local-enterprise-partnership-april-2017-lookup-in-england-v2)
* [Output Area to ... Ward Code](http://geoportal.statistics.gov.uk/datasets/output-area-to-ward-to-local-authority-district-december-2017-lookup-in-england-and-wales)
* [Output Area to NUTS region](https://geoportal1-ons.opendata.arcgis.com/datasets/a957f5205da24cb7bf22d3d1c671a7b1)

We process it in C# to create two tables,

1. Postcode to Output Area lookup.
2. Output Area to data lookup (there are over 30 columns in this table).

These are then served from a database (we use MySQL), via PHP, to an HTML5 app that process user input and displays the results.

A key part of our algorithm is assigning output areas to one of the following groups,
* Students and young pro's
* Poorer young families
* Wealthy young families
* Poorer older families
* Wealthy older families
* Poorer older people
* Wealthy older people

To do this we assign each output area one of low income, middle income, and high income -- based on their position within England & Wales' income distribution by small-area. We assign an age profile based on which of young children, older children, young adults, or older adults are most strongly over-represented in the data.

## Known issues.
The biggest issue that we know about with this tool comes from the need to set a baseline for comparison. To say whether a place is poorer or richer than average, we need to decide what average to use. We set our baseline using all the data available, and so this usually for the whole of England and Wales. When looking at national events this is fine, but if we're looking at events in just one place this be a bit misleading. For example, an event in West London may attract a representative mix of local people -- but our tool would report the audience as being above national average income and with an above average number of professionals (class AB). Similarly an event in East Yorkshire may attract a representative mix of local people -- but our tool would report this as being well below the national average for ethnic diversity. We have built local versions of our tool for Leeds and Bradford but for simplicity are not sharing them here. If you'd like such a tool, please get in touch.

Another issue is that we only know where people come from, and nothing more about them. The output of our tool is good for general demographic analysis, but should be improved with surveys at the venue to do much more.

## License.
The code for processing the raw data is licensed under the MIT license. Data is licensed under the [Open Government License v3 (OGL)](http://www.nationalarchives.gov.uk/doc/open-government-licence/version/3/), as it based on OGL licensed content itself.

## Funding.
This work was part-funded and supported by [Leeds Capital of Culture 2023](http://leeds2023.co.uk/) and Leeds City Council. It has been tested in Leeds and Bradford with Universities, small arts venues, and at The Open Data Institute Leeds.
