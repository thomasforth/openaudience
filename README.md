# Open Audience

Open Audience uses open data to analyse the demographics of your audience or users within England and Wales. Paste in a list of postcodes and see what type of people live there. You can also paste in Output Areas.

Try it at [openaudience.org](https://www.openaudience.org).

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

1. Baselines.
2. Output Area to data lookup (there are over 30 columns in this table).

We additionally use a Postcode to Output Area lookup.

These three files are then served from a database (we use MySQL), via PHP, to an HTML5 app that processes user input and displays the results. This web app is live at openaudience.org where you can see the source code. We may put it here at a later date (it's a mess now).

A key part of our algorithm is assigning output areas to one of the following groups,
* Students and young pro's
* Poorer young families
* Wealthy young families
* Poorer older families
* Wealthy older families
* Poorer older people
* Wealthy older people

To do this we assign each output area one of low income, middle income, and high income -- based on their position within England & Wales' income distribution by small-area. We assign an age profile based on which of young children, older children, young adults, or older adults are most strongly over-represented in the data, with a fudge factor on older adults to keep them from being over-represented.

## Known issues.
The biggest issue that we know about with this tool comes from the need to set a baseline for comparison. To say whether a place is poorer or richer than average, we need to decide what average to use. We set our baseline using all of the data available, and so this usually for the whole of England and Wales. When looking at national events this is fine, but if we're looking at events in just one place this be a bit misleading. For example, an event in West London may attract a representative mix of local people -- but our tool would report the audience as being above national average income and with an above average number of professionals (class AB). Similarly an event in East Yorkshire may attract a representative mix of local people -- but our tool would report this as being well below the national average for ethnic diversity. We have built local versions of our tool for Leeds and Bradford but for simplicity are not sharing them here. If you'd like such a tool, please get in touch.

Another issue is that we only know where people come from, and nothing more about them. The output of our tool is good for general demographic analysis, but should be improved with surveys at the venue to do much more.

A final issue is around sensitivity of language and description, especially where we make the extreme simplifications needed to have a useful tool.

## License.
The code for processing the raw data is licensed under the MIT license. Data is licensed under the [Open Government License v3 (OGL)](http://www.nationalarchives.gov.uk/doc/open-government-licence/version/3/), as it based on OGL licensed content itself.

## Funding and support.
This work was part-funded and supported by Leeds City Council as part of its efforts to understand cultural attendance right across the city and in a full range of venues, helping to inform the development and delivery of the new Culture Strategy for Leeds 2017-2030.[http://leeds2023.co.uk/].

We have had additional support from The Data City and this work is only possible by re-using open source components developed during projects with ODILeeds, The Future Cities Catapult, Arup, and The University of Leeds.

We have tested OpenAudience in Leeds and Bradford with Universities, small arts venues, and at The Open Data Institute Leeds. We regularly hear from users who are getting great results right across England and Wales.
