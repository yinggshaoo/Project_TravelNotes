using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection;
using System.Xml.Linq;
using System;
using static System.Collections.Specialized.BitVector32;

namespace TravelProject
{
    public class 測試資料
    {
    }
}


//< script >
//    function check(id)
//    {
//    var states = document.getElementsByName("labelGroup");
//    if (states)
//    {
//        states.checked = !states.checked; // 切換狀態
//        console.log("Check:", states.checked); // 打印狀態
//        console.log("Id:", id)
//        } else
//        {
//            console.log("元素未找到");
//        }

//        }
//</ script >






//< form method = "post" action = "~/Recommend/Index" >
//    < label for= "Bulbasaur " > 皮卡丘 </ label >
//    < input type = "checkbox" id = "Pikachu" name = "pokemon" value = "皮卡丘" >

//    < label for= "Bulbasaur " > 妙蛙種子 </ label >
//    < input type = "checkbox" id = "Bulbasaur" name = "pokemon" value = "妙蛙種子 " >

//    < label for= "Charmander  " > 小火龍 </ label >
//    < input type = "checkbox" id = "Charmander" name = "pokemon" value = "小火龍" >

//    < label for= "Charizard  " > 噴火龍 </ label >
//    < input type = "checkbox" id = "Charizard" name = "pokemon" value = "噴火龍" >

//    < label for= "Squirtle " > 傑尼龜 </ label >
//    < input type = "checkbox" id = "Squirtle" name = "pokemon" value = "傑尼龜" >

//    < button type = "submit" > Submit </ button >
//</ form >


//<form method="post" action="~/Recommend/Index" >
//    <div class="left-content-container">
//        <div class= "filter-container" >
//            <div class= "filter-title" > 路線 </div>
//            <div class= "filter-item-unlimited active" > 不限 </div>
//            <div class= "filter-data-wrap" >
//                @foreach (var item in Model)
//                {
//                    <input type = "checkbox" id = "@item.Id" name = "GetLabel"
//                    value = "@item.Label"/>
//                    <label for= "@item.Id" > @item.Label </label>
//                }
//            </div>
//        </div>
//        <input type = "submit" />
//    </div
//<form>