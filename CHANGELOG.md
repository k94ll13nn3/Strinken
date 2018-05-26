# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to[Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [3.3.0] - 2017-09-14

### Changed

- Remove net45 version.

### Fixed

- Fix incorrect enum visibility

## [3.2.1] - 2017-07-01

### Fixed 

- Fix regression : arguments were no longer passed to the filter validation process.

## [3.2.0] - 2017-06-18

### Added 

- New tag : Value tag
- New tags : Number tags
- Add base class for filter without arguments.

### Changed

- (Internal) New tokens registration and parsing mechanism.

### Issues

- [#43](https://github.com/k94ll13nn3/Strinken/issues/43): Add an abtract filter class that does not allow arguments
- [#42](https://github.com/k94ll13nn3/Strinken/issues/42): Number tags
- [#41](https://github.com/k94ll13nn3/Strinken/issues/41): Value tag

### Pull Requests

- [#40](https://github.com/k94ll13nn3/Strinken/pull/40): Parsing and tokens registration improvements (by [k94ll13nn3](https://github.com/k94ll13nn3))

## [3.1.0] - 2017-04-23

### Added 

- New filter : Repeat
- Add possibility to register and unregister base filters
- Add a Parser contructor that allows the creation of a parser without base filters

### Changed

- New project logo

### Issues

- [#38](https://github.com/k94ll13nn3/Strinken/issues/38): Repeat filter
- [#37](https://github.com/k94ll13nn3/Strinken/issues/37): Check filter type when unregistering it in the base filters list
- [#35](https://github.com/k94ll13nn3/Strinken/issues/35): Remove the state machine
- [#31](https://github.com/k94ll13nn3/Strinken/issues/31): Add a Parser contructor that allows the creation of a parser without base filters
- [#29](https://github.com/k94ll13nn3/Strinken/issues/29): Move to FluentAssertions for the tests
- [#28](https://github.com/k94ll13nn3/Strinken/issues/28): Move Strinken to the csproj format (when RTM)
- [#27](https://github.com/k94ll13nn3/Strinken/issues/27): Move to Wyam for the docs
- [#26](https://github.com/k94ll13nn3/Strinken/issues/26): Global filters registration
- [#14](https://github.com/k94ll13nn3/Strinken/issues/14): Update Cake to 0.16 and use .NET Core

### Pull Requests

- [#36](https://github.com/k94ll13nn3/Strinken/pull/36): Remove the state machine (by [k94ll13nn3](https://github.com/k94ll13nn3))
- [#34](https://github.com/k94ll13nn3/Strinken/pull/34): Move Strinken to the csproj format (by [k94ll13nn3](https://github.com/k94ll13nn3))
- [#33](https://github.com/k94ll13nn3/Strinken/pull/33): Simplify Travis build (by [k94ll13nn3](https://github.com/k94ll13nn3))
- [#32](https://github.com/k94ll13nn3/Strinken/pull/32): Add code coverage to build (by [k94ll13nn3](https://github.com/k94ll13nn3))
- [#30](https://github.com/k94ll13nn3/Strinken/pull/30): Move to FluentAssertions and xUnit for the tests (by [k94ll13nn3](https://github.com/k94ll13nn3))

## [3.0.0] - 2016-12-03

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

### Issues

- [#25](https://github.com/k94ll13nn3/Strinken/issues/25): Release version 3.0.0
- [#23](https://github.com/k94ll13nn3/Strinken/issues/23): Add possibility to compile a string
- [#17](https://github.com/k94ll13nn3/Strinken/issues/17): Update tests name
- [#16](https://github.com/k94ll13nn3/Strinken/issues/16): Replace filter
- [#12](https://github.com/k94ll13nn3/Strinken/issues/12): Update documentation for 3.0.0
- [#11](https://github.com/k94ll13nn3/Strinken/issues/11): Exclude comma from possible argument character
- [#9](https://github.com/k94ll13nn3/Strinken/issues/9): Validate tag and filter name when creating the parser
- [#8](https://github.com/k94ll13nn3/Strinken/issues/8): Add gh-pages site
- [#7](https://github.com/k94ll13nn3/Strinken/issues/7): Create non-generic tags

### Pull Requests

- [#24](https://github.com/k94ll13nn3/Strinken/pull/24): Add possibility to compile a string (by [k94ll13nn3](https://github.com/k94ll13nn3))
- [#22](https://github.com/k94ll13nn3/Strinken/pull/22): Add separate test project for public API (by [k94ll13nn3](https://github.com/k94ll13nn3))
- [#20](https://github.com/k94ll13nn3/Strinken/pull/20): Add replace filter (by [k94ll13nn3](https://github.com/k94ll13nn3))
- [#18](https://github.com/k94ll13nn3/Strinken/pull/18): Implementation of parameter tags (by [k94ll13nn3](https://github.com/k94ll13nn3))
- [#15](https://github.com/k94ll13nn3/Strinken/pull/15): Parser rewrite (by [k94ll13nn3](https://github.com/k94ll13nn3))
- [#10](https://github.com/k94ll13nn3/Strinken/pull/10): State machine and engine improvements (by [k94ll13nn3](https://github.com/k94ll13nn3))

## [2.0.0] - 2016-07-26

### Changed

- Rework parser creation
- Library now targets `netstandard1.0` instead of `netstandard1.1`
- Empty tag and filter names ared now considered as invalid by the builder.
- Empty or null string is now considered as valid by the parser.
- The validation method now returns a `ValidationResult`

### Issues

- [#6](https://github.com/k94ll13nn3/Strinken/issues/6): Handle empty tag/filter name as invalid
- [#4](https://github.com/k94ll13nn3/Strinken/issues/4): Add Travis build
- [#3](https://github.com/k94ll13nn3/Strinken/issues/3): Add documentation for ParserBuilder

### Pull Requests

- [#5](https://github.com/k94ll13nn3/Strinken/pull/5): Travis build (by [k94ll13nn3](https://github.com/k94ll13nn3))

## [1.0.1] - 2016-07-19

### Fixed

- Fix versioning

### Pull Requests

- [#2](https://github.com/k94ll13nn3/Strinken/pull/2): Fix semicolon (by [k94ll13nn3](https://github.com/k94ll13nn3))
- [#1](https://github.com/k94ll13nn3/Strinken/pull/1): Update ci (by [k94ll13nn3](https://github.com/k94ll13nn3))

## 1.0.0 - 2016-07-18

Initial release.

[3.3.0]: https://github.com/k94ll13nn3/Strinken/compare/v3.2.1...v3.3.0
[3.2.1]: https://github.com/k94ll13nn3/Strinken/compare/v3.2.0...v3.2.1
[3.2.0]: https://github.com/k94ll13nn3/Strinken/compare/v3.1.0...v3.2.0
[3.1.0]: https://github.com/k94ll13nn3/Strinken/compare/v3.0.0...v3.1.0
[3.0.0]: https://github.com/k94ll13nn3/Strinken/compare/v2.0.0...v3.0.0
[2.0.0]: https://github.com/k94ll13nn3/Strinken/compare/v1.0.1...v2.0.0
[1.0.1]: https://github.com/k94ll13nn3/Strinken/compare/v1.0.0...v1.0.1