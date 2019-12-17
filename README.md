# PhotoVs

## Engine Structure
- **PhotoVs.Utls**: contains purely basic consistent logic (such as math/basic data structures, loggers and extensions.
- **PhotoVs.Models**: contains all interfaces for everything between Engine/Logic and Events. Depends on *PhotoVs.Utils*.
- **PhotoVs.Engine**: contains all low-level implementations that don't refer to PhotoVs as a game. Depends on *PhotoVs.Utils* and *PhotoVs.Models*.
- **PhotoVs.Logic**: contains all logic specific to PhotoVs as a game. Depends on *PhotoVs.Utils*, *PhotoVs.Models*, and *PhotoVs.Engine*.
- **PhotoVs.Platform.Windows**: contains bootstrapping code to run PhotoVs on Microsoft Windows. Depends on *PhotoVs.Logic*.
