import * as React from "react";
import * as api from '../../common/api'
import { connect, IState, TypeOfConnect } from "../../store";
import { DialerInfo } from "../../common/api";
import { Box, Typography, TableContainer, Paper, Table, TableHead, TableRow, TableCell, TableBody, CircularProgress } from "@material-ui/core";

const storeEnhancer = connect((state: IState) => ({url: state.url}),{});

const DialersTab: React.SFC<TypeOfConnect<typeof storeEnhancer>> = (props) => {
  const [state, setState] = React.useState<{
        dialers: DialerInfo[], state: "ok"|"error"|"loading", error?: string
      }>({
        dialers:[], state:"loading"
      });

  React.useEffect(() => { (async () => {
    try{
      setState({ dialers: [], state:"loading"});
      const dialers = await api.getDialers();
      setState({ dialers, state:"ok"});
    }
    catch(error){
      setState({ dialers:[], state:"error", error: error.toString()});
    }
})()}, [props.url]);

  switch(state.state){
    case "ok":
      return (<Box>
        <TableContainer component={Paper}>
          <Table key="table" aria-label="simple table">
            <TableHead>
              <TableRow>
                <TableCell>CompanyId</TableCell>
                <TableCell>DialerId</TableCell>
                <TableCell align="right">Initialization Time</TableCell>
                <TableCell align="right">Agents count</TableCell>
                <TableCell align="right">Campaigns count</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {state.dialers.map((row) => (
                <TableRow key={row.companyId + ":" + row.dialerId}>
                  <TableCell component="th" scope="row">{row.companyId}</TableCell>
                  <TableCell component="th" scope="row">{row.dialerId}</TableCell>
                  <TableCell align="right">{row.initializationTime}</TableCell>
                  <TableCell align="right">{row.agentsCount}</TableCell>
                  <TableCell align="right">{row.campaignsCount}</TableCell>
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

export default storeEnhancer(DialersTab);