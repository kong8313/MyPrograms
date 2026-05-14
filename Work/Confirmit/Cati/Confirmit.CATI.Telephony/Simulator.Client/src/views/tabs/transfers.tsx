import * as React from "react";
import * as api from '../../common/api'
import { connect, IState, TypeOfConnect } from "../../store";
import { TransferInfo } from "../../common/api";
import { Box, Typography, TableContainer, Paper, Table, TableHead, TableRow, TableCell, TableBody, CircularProgress, makeStyles } from "@material-ui/core";

const useStyles = makeStyles({
  table: {
    minWidth: 650,
  },
});

const storeEnhancer = connect((state: IState) => ({url: state.url}),{});

const TransferTab: React.SFC<TypeOfConnect<typeof storeEnhancer>> = (props) => {
  const [state, setState] = React.useState<{
        transfers: TransferInfo[], state: "ok"|"error"|"loading", error?: string
      }>({
        transfers:[], state:"loading"
      });

  React.useEffect(() => { (async () => {
    try{
      setState({ transfers: [], state:"loading"});
      const transfers = await api.getTransfers();
      setState({ transfers, state:"ok"});
    }
    catch(error){
      setState({ transfers:[], state:"error", error: error.toString()});
    }
})()}, [props.url]);

  const classes = useStyles();
  switch(state.state){
    case "ok":
      return (<Box>
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Company Id</TableCell>
                <TableCell>Dialer Id</TableCell>
                <TableCell align="right">Transfer Id</TableCell>
                <TableCell align="right">Initiator</TableCell>
                <TableCell align="right">Target</TableCell>
                <TableCell align="right">Type</TableCell>
                <TableCell align="right">Route</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {state.transfers.map((row) => (
                <TableRow key={row.transferId}>
                  <TableCell component="th" scope="row">{row.companyId}</TableCell>
                  <TableCell component="th" scope="row">{row.dialerId}</TableCell>
                  <TableCell align="right">{row.transferId}</TableCell>
                  <TableCell align="right">{row.initiator}</TableCell>
                  <TableCell align="right">{row.target}</TableCell>
                  <TableCell align="right">{row.type}</TableCell>
                  <TableCell align="right">{row.state.connectionState}</TableCell>
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

export default storeEnhancer(TransferTab);