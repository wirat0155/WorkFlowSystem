using WorkFlowSystem.Services;

namespace WorkFlowSystem.Repository
{
    public class ApproveRepository
    {
        private readonly DapperService _dapper;

        public ApproveRepository(DapperService dapper)
        {
            _dapper = dapper;
        }
    }
}
