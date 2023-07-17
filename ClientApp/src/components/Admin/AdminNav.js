import React, { Component } from "react";
import "../../style/admin_nav.css"

export class AdminNav extends Component {
    render() {
        return (
            <div>
                <div className="p-3 mb-2 bg-light text-dark"  id="admin_nav">
                    <div id="logo">Adminstration</div>
                    <ul>
                        <li onClick={() => {window.location.href = "/admin_AdminCategories"}}>
                            Categories
                        </li>
                    </ul>
                </div>
            </div>
        )
    }
}