# Change Log

All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).
Thanks to [Keep a CHANGELOG](http://keepachangelog.com/) for the formating.

## 2.0.0 (2016-07-27)

### Changed

- Rework parser creation
- Library now targets `netstandard1.0` instead of `netstandard1.1`
- Empty tag and filter names ared now considered as invalid by the builder.
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