# Change Log

All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).
Thanks to [Keep a CHANGELOG](http://keepachangelog.com/) for the formating.

## 3.0.0 (2016-12-03)

### Added 

- New site for the project : https://k94ll13nn3.github.io/Strinken/
- New tag type : parameter tag
- New filter : Replace
- Add possibility to compile a string

### Changed

- Rework internal parsing engine

### Fixed 
- Invalid tag and filter names are now causing throwing an exception
- Exclude comma from possible argument character

## PR/Issues

 - [#24](https://github.com/k94ll13nn3/Strinken/pull/24) - Add possibility to compile a string contributed by Keuvain ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#23](https://github.com/k94ll13nn3/Strinken/issues/23) - Add possibility to compile a string +enhancement
 - [#22](https://github.com/k94ll13nn3/Strinken/pull/22) - Add separate test project for public API contributed by Keuvain ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#20](https://github.com/k94ll13nn3/Strinken/pull/20) - Add replace filter contributed by Keuvain ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#18](https://github.com/k94ll13nn3/Strinken/pull/18) - Implementation of parameter tags contributed by Keuvain ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#17](https://github.com/k94ll13nn3/Strinken/issues/17) - Update tests name
 - [#16](https://github.com/k94ll13nn3/Strinken/issues/16) - Replace filter
 - [#15](https://github.com/k94ll13nn3/Strinken/pull/15) - Parser rewrite contributed by Keuvain ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#12](https://github.com/k94ll13nn3/Strinken/issues/12) - Update documentation for 3.0.0
 - [#11](https://github.com/k94ll13nn3/Strinken/issues/11) - Exclude comma from possible argument character +fix
 - [#10](https://github.com/k94ll13nn3/Strinken/pull/10) - State machine and engine improvements contributed by Keuvain ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#9](https://github.com/k94ll13nn3/Strinken/issues/9) - Validate tag and filter name when creating the parser
 - [#8](https://github.com/k94ll13nn3/Strinken/issues/8) - Add gh-pages site
 - [#7](https://github.com/k94ll13nn3/Strinken/issues/7) - Create non-generic tags

## 2.0.0 (2016-07-26)

### Changed

- Rework parser creation
- Library now targets `netstandard1.0` instead of `netstandard1.1`
- Empty tag and filter names are now considered as invalid by the builder.
- Empty or null string is now considered as valid by the parser.
- The validation method now returns a `ValidationResult`

### PR/Issues

 - [#6](https://github.com/k94ll13nn3/Strinken/issues/6) - Handle empty tag/filter name as invalid
 - [#5](https://github.com/k94ll13nn3/Strinken/pull/5) - Travis build contributed by Keuvain ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#4](https://github.com/k94ll13nn3/Strinken/issues/4) - Add Travis build
 - [#3](https://github.com/k94ll13nn3/Strinken/issues/3) - Add documentation for ParserBuilder

## 1.0.1 (2016-07-19)

### Fixed

- Fix versioning

### PR/Issues

 - [#2](https://github.com/k94ll13nn3/Strinken/pull/2) - Fix semicolon contributed by Keuvain ([k94ll13nn3](https://github.com/k94ll13nn3))
 - [#1](https://github.com/k94ll13nn3/Strinken/pull/1) - Update ci contributed by Keuvain ([k94ll13nn3](https://github.com/k94ll13nn3))

## 1.0.0 (2016-07-18)

Initial release.