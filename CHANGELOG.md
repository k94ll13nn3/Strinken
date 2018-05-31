# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [4.0.0] - 2018-05-31

### Changed

- [**Breaking**] Change assembly versionning to be `Major.Minor` (instead of `Major.Minor.Patch`)
- [**Breaking**] Add a new property to `IFilter`: `AlternativeName`. This property can be used to set an alternative name composed of the following characters for the filter: `!`, `%`, `&`, `*`, `.`, `/`, `<`, `=`, `>`, `@`, `^`, `|`, `~`, `?`, `$` or `#`.
- [**Breaking**] All classes are now under the `Strinken` namespace only
- [**Breaking**] Reworked the resolution and compilation of a string. The compilation of a string now returns a `CompiledString` and is no longer stored in the parser. This `CompiledString` can be passed to an overload of `Resolve` for the resolution. Bulk resolution of values is now also possible.

### Fixed

- Fix a potential `NullReferenceException` in some base filters

### Issues

- [#54](https://github.com/k94ll13nn3/Strinken/issues/54): AssemblyVersion should be Major.Minor
- [#51](https://github.com/k94ll13nn3/Strinken/issues/51): Add possibility to register a filter under a new name
- [#49](https://github.com/k94ll13nn3/Strinken/issues/49): Consider bringing everything under the same namespace
- [#46](https://github.com/k94ll13nn3/Strinken/issues/46): Possible null reference exception in base tests
- [#45](https://github.com/k94ll13nn3/Strinken/issues/45): Add bulk resolution
- [#44](https://github.com/k94ll13nn3/Strinken/issues/44): Add possibility to compile more than one string

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
- [#35](https://github.com/k94ll13nn3/Strinken/issues/35): Remove the state machine
- [#31](https://github.com/k94ll13nn3/Strinken/issues/31): Add a Parser contructor that allows the creation of a parser without base filters
- [#27](https://github.com/k94ll13nn3/Strinken/issues/27): Move to Wyam for the docs
- [#26](https://github.com/k94ll13nn3/Strinken/issues/26): Global filters registration

### Pull Requests

- [#36](https://github.com/k94ll13nn3/Strinken/pull/36): Remove the state machine (by [k94ll13nn3](https://github.com/k94ll13nn3))

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

- [#23](https://github.com/k94ll13nn3/Strinken/issues/23): Add possibility to compile a string
- [#16](https://github.com/k94ll13nn3/Strinken/issues/16): Replace filter
- [#12](https://github.com/k94ll13nn3/Strinken/issues/12): Update documentation for 3.0.0
- [#11](https://github.com/k94ll13nn3/Strinken/issues/11): Exclude comma from possible argument character
- [#9](https://github.com/k94ll13nn3/Strinken/issues/9): Validate tag and filter name when creating the parser
- [#8](https://github.com/k94ll13nn3/Strinken/issues/8): Add gh-pages site
- [#7](https://github.com/k94ll13nn3/Strinken/issues/7): Create non-generic tags

### Pull Requests

- [#24](https://github.com/k94ll13nn3/Strinken/pull/24): Add possibility to compile a string (by [k94ll13nn3](https://github.com/k94ll13nn3))
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
- [#3](https://github.com/k94ll13nn3/Strinken/issues/3): Add documentation for ParserBuilder

## [1.0.1] - 2016-07-19

### Fixed

- Fix versioning

## 1.0.0 - 2016-07-18

Initial release.

[4.0.0]: https://github.com/k94ll13nn3/Strinken/compare/v3.3.0...v4.0.0
[3.3.0]: https://github.com/k94ll13nn3/Strinken/compare/v3.2.1...v3.3.0
[3.2.1]: https://github.com/k94ll13nn3/Strinken/compare/v3.2.0...v3.2.1
[3.2.0]: https://github.com/k94ll13nn3/Strinken/compare/v3.1.0...v3.2.0
[3.1.0]: https://github.com/k94ll13nn3/Strinken/compare/v3.0.0...v3.1.0
[3.0.0]: https://github.com/k94ll13nn3/Strinken/compare/v2.0.0...v3.0.0
[2.0.0]: https://github.com/k94ll13nn3/Strinken/compare/v1.0.1...v2.0.0
[1.0.1]: https://github.com/k94ll13nn3/Strinken/compare/v1.0.0...v1.0.1