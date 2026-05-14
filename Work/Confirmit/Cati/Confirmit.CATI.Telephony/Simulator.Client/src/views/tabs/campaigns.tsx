import * as React from "react";
import * as api from '../../common/api'
import { connect, IState, TypeOfConnect } from "../../store";
import { CampaignInfo } from "../../common/api";
import { Box, Typography, TableContainer, Paper, Table, TableHead, TableRow, TableCell, TableBody, CircularProgress, makeStyles } from "@material-ui/core";

const useStyles = makeStyles({
  table: {
    minWidth: 650,
  },
});

const storeEnhancer = connect((state: IState) => ({url: state.url}),{});

const CampaignTab: React.SFC<TypeOfConnect<typeof storeEnhancer>> = (props) => {
  const [state, setState] = React.useState<{
        campaigns: CampaignInfo[], state: "ok"|"error"|"loading", error?: string
      }>({
        campaigns:[], state:"loading"
      });

  React.useEffect(() => { (async () => {
    try{
      setState({ campaigns: [], state:"loading"});
      const campaigns = await api.getCampaigns();
      setState({ campaigns, state:"ok"});
    }
    catch(error){
      setState({ campaigns:[], state:"error", error: error.toString()});
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
                <TableCell align="right">Campaign Id</TableCell>
                <TableCell align="right">Name</TableCell>
                <TableCell align="right">Dialing Mode</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {state.campaigns.map((row) => (
                <TableRow key={row.campaignId}>
                  <TableCell component="th" scope="row">{row.companyId}</TableCell>
                  <TableCell component="th" scope="row">{row.dialerId}</TableCell>
                  <TableCell align="right">{row.campaignId}</TableCell>
                  <TableCell align="right">{row.name}</TableCell>
                  <TableCell align="right">{row.dialingMode}</TableCell>
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

export default storeEnhancer(CampaignTab);