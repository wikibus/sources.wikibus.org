# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/saintedlama/versionize) for commit guidelines.

<a name="0.12.1"></a>
## 0.12.1 (2020-5-3)

## 0.12.0 (2020-5-3)

### Bug Fixes

* page count not setting automatically

### Features

* admin operation to remove PDF

## 0.11.1 (2020-5-2)

### Bug Fixes

* user id has to be unescaped

## 0.11.0 (2020-5-2)

### Bug Fixes

* files were not correctly downloaded from google
* drive import should skip when no folders

### Features

* filtering brochures by contributor
* endpoint to get all contributors
* automatically set page count from pdf
* automatically imports brochures from google drive

## 0.10.0 (2020-4-25)

### Features

* operation to reorder images

## 0.9.0 (2020-4-19)

### Features

* link to contributing instructions

## 0.8.2 (2020-4-13)

### Bug Fixes

* avoid dereferencing schema.org context

## 0.8.1 (2020-4-13)

### Bug Fixes

* use full name instead of nickname

## 0.8.0 (2020-4-13)

### Bug Fixes

* user id is variable length
* only extract select page to save memory and space

### Features

* add user endpoint to azure functions
* add owner user to sources

## 0.7.0 (2020-4-11)

### Bug Fixes

* function ExtractPages should replace legacy image
* function ExtractPages would not save changes

### Features

* automatically extract cover page

## 0.6.0 (2020-4-5)

### Bug Fixes

* correctly find existing wishlist items

### Features

* brochure wishlist
* brochure wishlist

## 0.5.1 (2020-3-19)

### Bug Fixes

* facebook links are broken

## 0.5.0 (2020-3-19)

### Bug Fixes

* filter images did not work correctly
* updating brochure removes PDF

### Features

* added filters to find brochures with pdfs and without images

## 0.4.2 (2020-3-18)

### Bug Fixes

* anotar also missing from xsd

## 0.4.1 (2020-3-18)

### Bug Fixes

* missing Anotar reference in Images project

## 0.4.0 (2020-3-16)

### Bug Fixes

* disable kestrel file size limit
* return 404 when uploading image to missing brochure
* azure does not like slashes in container name
* azure does not like slashes in container name
* change the setting to increase file size
* mixup with request streams
* was redirect causing failed requests?
* do not use RowNumberForPaging
* the model does not like null int32 coming from DB
* unable to create brochure with no language

### Features

* uploading PDFs to brochure and converting pages to images

## 0.3.0 (2020-2-8)

### Features

* make the languages writeable

## 0.2 (2020-1-24)

### Bug Fixes

* first image thumbnail does not show
* first image thumbnail does not show
* missing cover image
* prevent auth being overriden
* creating brochure was impossible
* configuring nancy bootstrapper in the correct class
* do not drop headers on 404 responses
* missing permission check on imag upload
* add error logger pipeline
* better descriptions of entrypoint links
* require authentication of brochure updates
* do not do image resizing
* rdf/xml incorrectly serialized blank nodes
* representation shows image links where they should not

### Features

* saving storage location
* deleting images
* upload source images
* upload source images
* support multiple images for sources
* creating new brochure
* added entrypoint meta
* add a label to collection
* added manages block to collections
* updating brochure

