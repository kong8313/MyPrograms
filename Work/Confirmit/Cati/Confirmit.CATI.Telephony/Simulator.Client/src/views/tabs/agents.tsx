import * as React from "react";
import * as api from '../../common/api'
import { connect, IState, TypeOfConnect } from "../../store";
import { AgentInfo } from "../../common/api";
import { Box, Typography, TableContainer, Paper, Table, TableHead, TableRow, TableCell, TableBody, CircularProgress, Button } from "@material-ui/core";

const storeEnhancer = connect((state: IState) => ({url: state.url}),{});

const AgentsTab: React.SFC<TypeOfConnect<typeof storeEnhancer>> = (props) => {
  const [state, setState] = React.useState<{
        agents: AgentInfo[], state: "ok"|"error"|"loading", error?: string
      }>({
        agents:[], state:"loading"
      });

  React.useEffect(() => { (async () => {
    try{
      setState({ agents: [], state:"loading"});
      const agents = await api.getAgents();
      setState({ agents, state:"ok"});
    }
    catch(error){
      setState({ agents:[], state:"error", error: error.toString()});
    }
})()}, [props.url]);

  const logoutAgent = (companyId: number, dialerId: number, agentId: number) => {
    try{
      api.deleteAgent(companyId, dialerId, agentId);
    }catch(err){

    }
  }

  switch(state.state){
    case "ok":
      return (<Box>
        <TableContainer component={Paper}>
          <Table key="table" aria-label="simple table">
            <TableHead>
              <TableRow>
                <TableCell>CompanyId</TableCell>
                <TableCell>DialerId</TableCell>
                <TableCell>Name</TableCell>
                <TableCell align="right">campaignId</TableCell>
                <TableCell align="right">Predicitve</TableCell>
                <TableCell align="right">AgentId</TableCell>
                <TableCell align="right">Type</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {state.agents.map((row) => (
                <TableRow key={row.name}>
                  <TableCell component="th" scope="row">{row.companyId}</TableCell>
                  <TableCell component="th" scope="row">{row.dialerId}</TableCell>
                  <TableCell component="th" scope="row">{row.name}</TableCell>
                  <TableCell align="right">{row.campaignId}</TableCell>
                  <TableCell align="right">{row.isPredictive ? "Yes" : "No"}</TableCell>
                  <TableCell align="right">{row.agentId}</TableCell>
                  <TableCell align="right">{row.type}</TableCell>
                  <TableCell align="right">
                    <Button onClick={e => logoutAgent(row.companyId, row.dialerId, row.agentId)} variant="contained" 
                          color="primary">Logout</Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Box>);
    case "loading":
      return (<Box display="flex"><CircularProgress /></Box>);
    case "error":
    default:
      return (<Box><Typography color="error">{state?.error ?? "Unknown error"}</Typography></Box>)
  }
}

export default storeEnhancer(AgentsTab);