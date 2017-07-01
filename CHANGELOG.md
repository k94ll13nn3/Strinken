# Change Log

All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).
Thanks to [Keep a CHANGELOG](http://keepachangelog.com/) for the formating.

## 3.0.1 (2017-07-01)

### Fixed 

- Fix regression : arguments were no longer passed to the filter validation process.

## 3.2.0 (2017-06-18)

### Added 

- New tag : Value tag
- New tags : Number tags
- Add base class for filter without arguments.

### Changed

- (Internal) New tokens registration and parsing mechanism.

## PR/Issues

 - [#43](https://github.com/k94ll13nn3/Strinken/issues/43) - Add an abtract filter class that does not allow arguments +enhancement
 - [#42](https://github.com/k94ll13nn3/Strinken/issues/42) - Number tags
 - [#41](https://github.com/k94ll13nn3/Strinken/issues/41) - Value tag
 - [#40](https://github.com/k94ll13nn3/Strinken/pull/40) - Parsing and tokens registration improvements contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))

Commits: e8d00955e9...89d5ee10e8

## 3.1.0 (2017-04-23)

### Added 

- New filter : Repeat
- Add possibility to register and unregister base filters
- Add a Parser contructor that allows the creation of a parser without base filters

### Changed

- New project logo

## PR/Issues

 - [#38](https://github.com/k94ll13nn3/Strinken/issues/38) - Repeat filter
 - [#37](https://github.com/k94ll13nn3/Strinken/issues/37) - Check filter type when unregistering it in the base filters list
 - [#36](https://github.com/k94ll13nn3/Strinken/pull/36) - Remove the state machine contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#35](https://github.com/k94ll13nn3/Strinken/issues/35) - Remove the state machine
 - [#34](https://github.com/k94ll13nn3/Strinken/pull/34) - Move Strinken to the csproj format contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#33](https://github.com/k94ll13nn3/Strinken/pull/33) - Simplify Travis build contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#32](https://github.com/k94ll13nn3/Strinken/pull/32) - Add code coverage to build contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#31](https://github.com/k94ll13nn3/Strinken/issues/31) - Add a Parser contructor that allows the creation of a parser without base filters +enhancement
 - [#30](https://github.com/k94ll13nn3/Strinken/pull/30) - Move to FluentAssertions and xUnit for the tests contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#29](https://github.com/k94ll13nn3/Strinken/issues/29) - Move to FluentAssertions for the tests
 - [#28](https://github.com/k94ll13nn3/Strinken/issues/28) - Move Strinken to the csproj format (when RTM)
 - [#27](https://github.com/k94ll13nn3/Strinken/issues/27) - Move to Wyam for the docs
 - [#26](https://github.com/k94ll13nn3/Strinken/issues/26) - Global filters registration +enhancement
 - [#25](https://github.com/k94ll13nn3/Strinken/issues/25) - Release version 3.0.0
 - [#14](https://github.com/k94ll13nn3/Strinken/issues/14) - Update Cake to 0.16 and use .NET Core

Commits: 83023996c4...2e86a9c314

## 3.0.0 (2016-12-03)

### Added 

- New site for the project : https://k94ll13nn3.github.io/Strinken/
- New tag type : parameter tag
- New filter : Replace
- Add possibility to compile a string

### Changed

- Rework internal parsing engine

### Fixed 
- Invalid tag and filter names are now throwing an exception
- Exclude comma from possible argument character

## PR/Issues

 - [#24](https://github.com/k94ll13nn3/Strinken/pull/24) - Add possibility to compile a string contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#23](https://github.com/k94ll13nn3/Strinken/issues/23) - Add possibility to compile a string +enhancement
 - [#22](https://github.com/k94ll13nn3/Strinken/pull/22) - Add separate test project for public API contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#21](https://github.com/k94ll13nn3/Strinken/pull/21) - Add build messages to AppVeyor message tab contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#20](https://github.com/k94ll13nn3/Strinken/pull/20) - Add replace filter contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#19](https://github.com/k94ll13nn3/Strinken/pull/19) - Move to Cake.CoreCLR contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#18](https://github.com/k94ll13nn3/Strinken/pull/18) - Implementation of parameter tags contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#17](https://github.com/k94ll13nn3/Strinken/issues/17) - Update tests name
 - [#16](https://github.com/k94ll13nn3/Strinken/issues/16) - Replace filter
 - [#15](https://github.com/k94ll13nn3/Strinken/pull/15) - Parser rewrite contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#13](https://github.com/k94ll13nn3/Strinken/pull/13) - Implementation of parameter tags contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#12](https://github.com/k94ll13nn3/Strinken/issues/12) - Update documentation for 3.0.0
 - [#11](https://github.com/k94ll13nn3/Strinken/issues/11) - Exclude comma from possible argument character +fix
 - [#10](https://github.com/k94ll13nn3/Strinken/pull/10) - State machine and engine improvements contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#9](https://github.com/k94ll13nn3/Strinken/issues/9) - Validate tag and filter name when creating the parser
 - [#8](https://github.com/k94ll13nn3/Strinken/issues/8) - Add gh-pages site
 - [#7](https://github.com/k94ll13nn3/Strinken/issues/7) - Create non-generic tags

Commits: 5b3b83e5ff...65a096f2f2

## 2.0.0 (2016-07-26)

### Changed

- Rework parser creation
- Library now targets `netstandard1.0` instead of `netstandard1.1`
- Empty tag and filter names are now considered as invalid by the builder.
- Empty or null string is now considered as valid by the parser.
- The validation method now returns a `ValidationResult`

### PR/Issues

 - [#6](https://github.com/k94ll13nn3/Strinken/issues/6) - Handle empty tag/filter name as invalid
 - [#5](https://github.com/k94ll13nn3/Strinken/pull/5) - Travis build contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#4](https://github.com/k94ll13nn3/Strinken/issues/4) - Add Travis build
 - [#3](https://github.com/k94ll13nn3/Strinken/issues/3) - Add documentation for ParserBuilder

Commits: 04474cbd56...e59a7be0d3

## 1.0.1 (2016-07-19)

### Fixed

- Fix versioning

### PR/Issues

 - [#2](https://github.com/k94ll13nn3/Strinken/pull/2) - Fix semicolon contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#1](https://github.com/k94ll13nn3/Strinken/pull/1) - Update ci contributed by Kévin Gallienne ([k94ll13nn3](https://github.com/k94ll13nn3))

Commits: daacaa2b63...bbe65b7322

## 1.0.0 (2016-07-18)

Initial release.
