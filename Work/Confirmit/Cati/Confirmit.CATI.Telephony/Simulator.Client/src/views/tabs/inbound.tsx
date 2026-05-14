import * as React from "react";
import * as api from '../../common/api'
import { connect, IState, TypeOfConnect } from "../../store";
import { InboundDdiInfo, InboundCallInfo } from "../../common/api";
import { Box, Typography, TableContainer, Paper, Table, TableHead, TableRow, TableCell, TableBody, CircularProgress, Button, TextField } from "@material-ui/core";
import useInterval from 'react-useinterval';

const storeEnhancer = connect((state: IState) => ({url: state.url}),{});

const InboundTab: React.SFC<TypeOfConnect<typeof storeEnhancer>> = (props) => {
  const [state, setState] = React.useState<{
        ddi: InboundDdiInfo[], calls: InboundCallInfo[], state: "ok"|"error"|"loading", error?: string
      }>({
        ddi:[], calls:[], state:"loading"
      });
  const [clis, setClis] = React.useState({});
  const createKey = (companyId: number, dialerId: number, number: string) => companyId + ":" + dialerId + ":" + number;
  const createInboundCall = (companyId: number, dialerId: number, ddiNumber: string, cliNumber: string) => {
    try {
      api.createInboundCall({companyId, dialerId, ddiNumber, cliNumber})
    }catch(error){}
  }

  const dropInboundCall = (companyId: number, dialerId: number, inboundCallId: string) => {
    try {
      api.dropInboundCall(companyId, dialerId, inboundCallId)
    }catch(error){}
  }

  useInterval( () => (async () => {
    try{
      const ddi = await api.getInboundDDI();
      const calls = await api.getInboundCalls();
      setState({ ddi, calls, state:"ok"});
    }
    catch(error){
      setState({ ddi: [], calls: [], state:"error", error: error.toString()});
    }
  })(), 500 );

  switch(state.state){
    case "ok":
      return (<Box>
        <TableContainer component={Paper}>
          <Table key="table" aria-label="simple table">
            <TableHead>
              <TableRow>
                <TableCell>CompanyId</TableCell>
                <TableCell>DialerId</TableCell>
                <TableCell align="right">ID</TableCell>
                <TableCell align="right">DDI Number</TableCell>
                <TableCell align="right">CLI Number</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {state.ddi.map((row) => {
                const key = createKey(row.companyId, row.dialerId, row.number)
                return (
                <TableRow key={key}>
                  <TableCell component="th" scope="row">{row.companyId}</TableCell>
                  <TableCell component="th" scope="row">{row.dialerId}</TableCell>
                  <TableCell align="right"></TableCell>
                  <TableCell align="right">{row.number}</TableCell>
                  <TableCell align="right"><TextField value={clis[key]} onChange={value => setClis({...clis, [key]: value.target.value})}/></TableCell>
                  <TableCell align="right">
                      <Button key={row.companyId + ":" + row.dialerId} variant="contained" 
                          color="primary" 
                          onClick={() => createInboundCall(row.companyId, row.dialerId, row.number, clis[key])}>
                        Create Call
                      </Button>
                  </TableCell>
                </TableRow>)})}
              {state.calls.map((row) => {
                const key = createKey(row.companyId, row.dialerId, row.inboundCallId)
                return (
                <TableRow key={key}>
                  <TableCell component="th" scope="row">{row.companyId}</TableCell>
                  <TableCell component="th" scope="row">{row.dialerId}</TableCell>
                  <TableCell align="right">{row.inboundCallId}</TableCell>
                  <TableCell align="right">{row.ddiNumber}</TableCell>
                  <TableCell align="right">{row.cliNumber}</TableCell>
                  <TableCell align="right">
                      <Button key={row.companyId + ":" + row.dialerId} variant="contained" 
                          color="primary" 
                          onClick={() => dropInboundCall(row.companyId, row.dialerId, row.inboundCallId)}>
                        Drop call
                      </Button>
                  </TableCell>
                </TableRow>)})}
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

export default storeEnhancer(InboundTab);