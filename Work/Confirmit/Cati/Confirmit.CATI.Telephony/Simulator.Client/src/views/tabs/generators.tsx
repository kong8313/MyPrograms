import * as React from "react";
import * as api from '../../common/api'
import { connect, IState, Actions, TypeOfConnect } from "../../store";
import { GeneratorInfo, GeneratorBehaviorInfo } from "../../common/api";
import { Box, Typography, TableContainer, Paper, Table, TableHead, TableRow, TableCell, TableBody, IconButton, Dialog } from "@material-ui/core";
import AddIcon from '@material-ui/icons/Add';
import DeleteIcon from '@material-ui/icons/Delete';
import {withRouter, RouteComponentProps, useHistory} from 'react-router-dom'
import { routes } from ".";
import './generators.scss'

const storeEnhancer = connect(
  (state: IState) => ({url: state.url}),
  {
    configure: Actions.configure,
  },
);

type Props = TypeOfConnect<typeof storeEnhancer>;


const getFilter = (behavior: GeneratorBehaviorInfo) => {
  let result: {field: string, value: string}[] = [];
  if(behavior.filter?.companyId)result.push({ field: "companyId", value: behavior.filter.companyId.toString()} );
  if(behavior.filter?.dialerId)result.push({ field: "dialerId", value: behavior.filter.dialerId.toString()} );
  if(behavior.filter?.campaignId)result.push({ field: "campaignId", value: behavior.filter.campaignId.toString()} );
  if(behavior.filter?.agentId)result.push({ field: "agentId", value: behavior.filter.agentId.toString()} );
  if(behavior.filter?.interviewId)result.push({ field: "interviewId", value: behavior.filter.interviewId.toString()} );

  return <>{result.map((x,i) => (
    <span>
        <span className="filter-text">{i == 0 ? "" : ", "}</span> 
        <span className="filter-field">{x.field}</span>
        <span className="filter-text"> = </span>
        <span className="filter-value">{x.value}</span>
      </span>))}</>
};
const getGeneratorName = (generator: GeneratorInfo) => {
  return (
  <span>
    <span className="filter-field">{generator.name}</span>
    <span className="filter-text"> => </span>
    <span className="filter-value">{generator.type}</span>
  </span>)
}
const getGeneratorBehevior = (behavior?: GeneratorBehaviorInfo) => {
  if(!behavior)
    return (<span className="filter-text">Default</span>)
  if (behavior.type) {
    return (<span>
      <span className="filter-field">Value</span>
      <span className="filter-text"> = </span>
      <span className="filter-value">{behavior.value}</span>
    </span>)
  }
  return (<span className="filter-field">Manual</span>);
}

const GeneratorsTab: React.SFC<Props & RouteComponentProps> = (props) => {
  const history = useHistory();
  const [state, setState] = React.useState<{
        generators: GeneratorInfo[], state: "ok"|"error"|"loading", error?: string
      }>({
        generators:[], state:"loading"
      });
  React.useEffect(() => { (async () => {
    try{
      setState({ generators: [], state:"loading"});
      const generators = await api.getGenerators();
      setState({ generators: generators, state:"ok"});
    }
    catch(error){
      setState({ generators:[], state:"error", error: error.toString()});
    }
})()}, [props.url]);

  const deleteBehavior = (name: string, id: string) => {
    api.deleteGeneratorBehavior(name, id);
    history.go();
  };

  switch(state.state){
    case "ok":
      return (<Box>
          <TableContainer component={Paper}>
          <Table stickyHeader={true}>
            <TableHead>
              <TableRow>
                <TableCell>Generator name</TableCell>
                <TableCell>Behavior</TableCell>
                <TableCell align="right">Filter</TableCell>
                <TableCell align="right">Owner</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {state.generators.map((generator) => [(
                <TableRow key={generator.name}>
                  <TableCell rowSpan={1 + generator.behaviors.length } component="th" scope="row">{getGeneratorName(generator)}</TableCell>
                  <TableCell>{getGeneratorBehevior(null)}</TableCell>
                  <TableCell align="right"></TableCell>
                  <TableCell align="right"></TableCell>
                  <TableCell align="right">
                    <IconButton onClick={()=> props.history.push(routes.addGeneratorBehavior.link(generator.name))} size="small"><AddIcon/></IconButton>
                  </TableCell>
                </TableRow>), 
                ...generator.behaviors.map(behavior => (
                  <TableRow key={behavior.id}>
                    <TableCell>{getGeneratorBehevior(behavior)}</TableCell>
                    <TableCell align="right">{getFilter(behavior)}</TableCell>
                    <TableCell align="right">{behavior.owner}</TableCell>
                    <TableCell align="right">
                      <IconButton  size="small" onClick={()=> {deleteBehavior(generator.name, behavior.id)} }><DeleteIcon/></IconButton>
                    </TableCell>
                  </TableRow>
                ))]
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Box>);
    case "loading":
      return (<Box><Typography>Loading ...</Typography></Box>);
    case "error":
    default:
      return (<Box><Typography color="error">{state?.error ?? "Unknown error"}</Typography></Box>)
  }
}

export default withRouter(storeEnhancer(GeneratorsTab));