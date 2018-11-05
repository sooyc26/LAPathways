import React from 'react';
import { FormGroup, ControlLabel, FormControl, HelpBlock } from 'react-bootstrap';
import axios from "axios"
import * as validation from './../utils/validation';
import Select from 'react-select'

export default class UserTypesDropdown extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            typeList: []
        }
        this.readAll=this.readAll.bind(this);

    }

    componentDidMount(){
        this.readAll();
    }

    readAll() {
        const url = "/api/user-types";
        
        const promise = axios.get(url)
        return promise
            .then((response)=>{
                const typeList = response.data.items;
                this.setState({typeList:typeList});
                console.log("checking type "+JSON.stringify(typeList));
            })

            .catch(console.log.error);
    }

    render(){

        //mapping list of types from user_types DB
        const listTypes = this.state.typeList.map((type,index)=>{
            return(
                <option style={{backgroundColor:"black"}} value={index+1} >
                {type.typeName}
                </option>
            )
        })
        return(
            <span>

                <FormGroup size="" controlId="formControlsSelect">
                    <ControlLabel>User Type</ControlLabel>

                    <Select 
                        options={this.props.options}                        
                    />

                    <select name="userTypeId" 
                    value={this.props.userTypeId.value} 
                    onChange={this.props.onChange} multiple="" tabindex="-1" aria-hidden="true"
                        className={(this.props.userTypeId.value === '0' || this.props.userTypeId.value === '') ?
                            'form-control' : 'form-control is-valid'}
                    >
                        <option style={{ backgroundColor: "black"}} value='' >select type</option>
                        {listTypes}
                    </select>
                    
                    <FormControl.Feedback />
                    {this.props.userTypeId.input && !validation.integer(this.props.userTypeId.value) ? <HelpBlock>Please select type</HelpBlock> : null}
                </FormGroup>
            </span>
        )
    }

}