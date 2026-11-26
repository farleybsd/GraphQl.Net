using Domain.Heroes;
using Domain.Teams;

namespace WebApi.Common.Persistence;

// TODO: Novos IDs fortemente tipados devem ser registrados aqui

[EfCoreConverter<HeroId>]
[EfCoreConverter<TeamId>]
[EfCoreConverter<MissionId>]
internal sealed partial class VogenEfCoreConverters;
