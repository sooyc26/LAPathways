import React from 'react'
import '../layout/PageTransition.css'

/*HOW TO USE
Load <LoaindSpinner> into component render() return that you want.
Passdown loading props from parent state to child(create in state as necessary)
set this.state.loading T/F from parent state to trigger visibility of spinner 
*/ 
export default class LoadingSpinner extends React.Component{
    constructor(props){
        super(props);

    }

    render() {
        return (
            <div>
                {/* loading spinner */}
                <div className={this.props.loading ? "page-loader" : "page-loader-hidden"} >
                    <div className="page-loader__spinner">
                        <svg viewBox="25 25 50 50">
                            <circle cx="50" cy="50" r="20" fill="none" strokeWidth="2" strokeMiterlimit="10" />
                        </svg>
                    </div>
                </div>
            </div>
        )
    }
}