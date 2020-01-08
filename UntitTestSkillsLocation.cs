using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gride.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrideTest
{
    [TestClass]
    public class UntitTestSkillsLocation
    {
        [TestMethod]
        public void Skill_Emoji_Length()
        {
            Skill skills = new Skill();
            string expected = "😀 😁 😂 🤣 😃 😄 😅 😆 😉 😊 😋 😎 😍 😘 🥰 😗 😙 😚 ☺️ 🙂 🤗 🤩 🤔 🤨 😐 😑 😶 🙄 😏 😣 😥 😮 🤐 😯 😪 😫 😴 😌 😛 😜 😝 🤤 😒 😓 😔 😕 🙃 🤑 😲 ☹️ 🙁 😖 😞 😟 😤 😢 😭 😦 😧 😨 😩 🤯 😬 😰 😱 🥵 🥶 😳 🤪 😵 😡 😠 🤬 😷 🤒 🤕 🤢 🤮 🤧 😇 🤠 🤡 🥳 🥴 🥺 🤥 🤫 🤭 🧐 🤓 😈 👿 👹 👺 💀 👻 👽 🤖 💩 😺 😸 😹 😻 😼 😽 🙀 😿 😾";
            skills.Name = "😀 😁 😂 🤣 😃 😄 😅 😆 😉 😊 😋 😎 😍 😘 🥰 😗 😙 😚 ☺️ 🙂 🤗 🤩 🤔 🤨 😐 😑 😶 🙄 😏 😣 😥 😮 🤐 😯 😪 😫 😴 😌 😛 😜 😝 🤤 😒 😓 😔 😕 🙃 🤑 😲 ☹️ 🙁 😖 😞 😟 😤 😢 😭 😦 😧 😨 😩 🤯 😬 😰 😱 🥵 🥶 😳 🤪 😵 😡 😠 🤬 😷 🤒 🤕 🤢 🤮 🤧 😇 🤠 🤡 🥳 🥴 🥺 🤥 🤫 🤭 🧐 🤓 😈 👿 👹 👺 💀 👻 👽 🤖 💩 😺 😸 😹 😻 😼 😽 🙀 😿 😾";
            Assert.AreEqual(skills.Name, expected, "Skills Emoji Length is True");
        }
        [TestMethod]
        public void Skill_Latin_Length()
        {
            Skill skills = new Skill();
            string expected = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
            skills.Name = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
            Assert.AreEqual(skills.Name, expected, "Skills Latin Length is True");
        }
        [TestMethod]
        public void Skill_Arabic()
        {
            Skill skills = new Skill();
            string expected = "اجميع الناس أحرارًا متساوين في الكرامة والحقوق. وقد وهبوا عقلاً وضميرًا وعليهم أن يعامل بعضهم بعضًا بروح الإخاء. 2 لكل إنسان حق التمتع بكافة الحقوق والحريات الواردة في هذا الإعلان، دون أي تمييز، كالتمييز بسبب العنصر أو اللون أو الجنس أو اللغة أو الدين أو امييز أساسهلأو تلك البقعة مستقلاً أو تحت الوصاية أو غير متمتع بالحكم الذاتي أو كانت سيايود.";
            skills.Name = "اجميع الناس أحرارًا متساوين في الكرامة والحقوق. وقد وهبوا عقلاً وضميرًا وعليهم أن يعامل بعضهم بعضًا بروح الإخاء. 2 لكل إنسان حق التمتع بكافة الحقوق والحريات الواردة في هذا الإعلان، دون أي تمييز، كالتمييز بسبب العنصر أو اللون أو الجنس أو اللغة أو الدين أو امييز أساسهلأو تلك البقعة مستقلاً أو تحت الوصاية أو غير متمتع بالحكم الذاتي أو كانت سيايود.";
            Assert.AreEqual(skills.Name, expected, "Skills Arabic is True");
        }
        [TestMethod]
        public void Skill_Russian()
        {
            Skill skills = new Skill();
            string expected = "Не́которые иностра́нцы ду́мают, что в Росси́и медве́ди хо́дят по у́лицам. Коне́чно, э́то непра́вда! Медве́ди живу́т в лесу́ и не лю́бят люде́й.";
            skills.Name = "Не́которые иностра́нцы ду́мают, что в Росси́и медве́ди хо́дят по у́лицам. Коне́чно, э́то непра́вда! Медве́ди живу́т в лесу́ и не лю́бят люде́й.";
            Assert.AreEqual(skills.Name, expected, "Skills Russian is True");
        }
        [TestMethod]
        public void Skill_Digit_Length()
        {
            Skill skills = new Skill();
            string expected = "3.141592653589793238462643383279502884197169399375105820974944592307816406286 208998628034825342117067982148086513282306647093844609550582231725359408128481 117450284102701938521105559644622948954930381964428810975665933446128475648233 786783165271201909145648566923460348610454326648213393607260249141273724587006 606315588174881520920962829254091715364367892590360011330530548820466521384146 951941511609433057270365759591953092186117381932611793105118548074462379962749";
            skills.Name = "3.141592653589793238462643383279502884197169399375105820974944592307816406286 208998628034825342117067982148086513282306647093844609550582231725359408128481 117450284102701938521105559644622948954930381964428810975665933446128475648233 786783165271201909145648566923460348610454326648213393607260249141273724587006 606315588174881520920962829254091715364367892590360011330530548820466521384146 951941511609433057270365759591953092186117381932611793105118548074462379962749";
            Assert.AreEqual(skills.Name, expected, "Skills Digits Length is True");
        }
        [TestMethod]
        public void Skill_Space()
        {
            Skill skills = new Skill();
            string expected = "                                    ";
            skills.Name = "                                    ";
            Assert.AreEqual(skills.Name, expected, "Skills Space is True");
        }
        [TestMethod]
        public void Skill_Null()
        {
            Skill skills = new Skill();
            string expected = null;
            skills.Name = null;
            Assert.AreEqual(skills.Name, expected, "Skills Null is True");
        }
        [TestMethod]
        public void Skill_0()
        {
            Skill skills = new Skill();
            string expected = "\0";
            skills.Name = "\0";
            Assert.AreEqual(skills.Name, expected, "Skills Null is True");
        }

    }

    [TestClass]
    public class UnitTestLocationTest
    {
        [TestMethod]
        public void Location_Emoji_Length()
        {
            Location locations = new Location();
            string expected = "😀 😁 😂 🤣 😃 😄 😅 😆 😉 😊 😋 😎 😍 😘 🥰 😗 😙 😚 ☺️ 🙂 🤗 🤩 🤔 🤨 😐 😑 😶 🙄 😏 😣 😥 😮 🤐 😯 😪 😫 😴 😌 😛 😜 😝 🤤 😒 😓 😔 😕 🙃 🤑 😲 ☹️ 🙁 😖 😞 😟 😤 😢 😭 😦 😧 😨 😩 🤯 😬 😰 😱 🥵 🥶 😳 🤪 😵 😡 😠 🤬 😷 🤒 🤕 🤢 🤮 🤧 😇 🤠 🤡 🥳 🥴 🥺 🤥 🤫 🤭 🧐 🤓 😈 👿 👹 👺 💀 👻 👽 🤖 💩 😺 😸 😹 😻 😼 😽 🙀 😿 😾";
            locations.Name = "😀 😁 😂 🤣 😃 😄 😅 😆 😉 😊 😋 😎 😍 😘 🥰 😗 😙 😚 ☺️ 🙂 🤗 🤩 🤔 🤨 😐 😑 😶 🙄 😏 😣 😥 😮 🤐 😯 😪 😫 😴 😌 😛 😜 😝 🤤 😒 😓 😔 😕 🙃 🤑 😲 ☹️ 🙁 😖 😞 😟 😤 😢 😭 😦 😧 😨 😩 🤯 😬 😰 😱 🥵 🥶 😳 🤪 😵 😡 😠 🤬 😷 🤒 🤕 🤢 🤮 🤧 😇 🤠 🤡 🥳 🥴 🥺 🤥 🤫 🤭 🧐 🤓 😈 👿 👹 👺 💀 👻 👽 🤖 💩 😺 😸 😹 😻 😼 😽 🙀 😿 😾";
            Assert.AreEqual(locations.Name, expected, "Locations Emoji Length is True");
        }
        [TestMethod]
        public void Location_Latin_Length()
        {
            Location locations = new Location();
            string expected = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
            locations.Name = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
            Assert.AreEqual(locations.Name, expected, "Locations Latin Length is True");
        }
        [TestMethod]
        public void Location_Arabic()
        {
            Location locations = new Location();
            string expected = "اجميع الناس أحرارًا متساوين في الكرامة والحقوق. وقد وهبوا عقلاً وضميرًا وعليهم أن يعامل بعضهم بعضًا بروح الإخاء. 2 لكل إنسان حق التمتع بكافة الحقوق والحريات الواردة في هذا الإعلان، دون أي تمييز، كالتمييز بسبب العنصر أو اللون أو الجنس أو اللغة أو الدين أو امييز أساسهلأو تلك البقعة مستقلاً أو تحت الوصاية أو غير متمتع بالحكم الذاتي أو كانت سيايود.";
            locations.Name = "اجميع الناس أحرارًا متساوين في الكرامة والحقوق. وقد وهبوا عقلاً وضميرًا وعليهم أن يعامل بعضهم بعضًا بروح الإخاء. 2 لكل إنسان حق التمتع بكافة الحقوق والحريات الواردة في هذا الإعلان، دون أي تمييز، كالتمييز بسبب العنصر أو اللون أو الجنس أو اللغة أو الدين أو امييز أساسهلأو تلك البقعة مستقلاً أو تحت الوصاية أو غير متمتع بالحكم الذاتي أو كانت سيايود.";
            Assert.AreEqual(locations.Name, expected, "Locations Arabic is True");
        }
        [TestMethod]
        public void Location_Russian()
        {
            Location locations = new Location();
            string expected = "Не́которые иностра́нцы ду́мают, что в Росси́и медве́ди хо́дят по у́лицам. Коне́чно, э́то непра́вда! Медве́ди живу́т в лесу́ и не лю́бят люде́й.";
            locations.Name = "Не́которые иностра́нцы ду́мают, что в Росси́и медве́ди хо́дят по у́лицам. Коне́чно, э́то непра́вда! Медве́ди живу́т в лесу́ и не лю́бят люде́й.";
            Assert.AreEqual(locations.Name, expected, "Locations Russian is True");
        }
        [TestMethod]
        public void Location_Digit_Length()
        {
            Location locations = new Location();
            string expected = "3.141592653589793238462643383279502884197169399375105820974944592307816406286 208998628034825342117067982148086513282306647093844609550582231725359408128481 117450284102701938521105559644622948954930381964428810975665933446128475648233 786783165271201909145648566923460348610454326648213393607260249141273724587006 606315588174881520920962829254091715364367892590360011330530548820466521384146 951941511609433057270365759591953092186117381932611793105118548074462379962749";
            locations.Name = "3.141592653589793238462643383279502884197169399375105820974944592307816406286 208998628034825342117067982148086513282306647093844609550582231725359408128481 117450284102701938521105559644622948954930381964428810975665933446128475648233 786783165271201909145648566923460348610454326648213393607260249141273724587006 606315588174881520920962829254091715364367892590360011330530548820466521384146 951941511609433057270365759591953092186117381932611793105118548074462379962749";
            Assert.AreEqual(locations.Name, expected, "Locations Digits Length is True");
        }
        [TestMethod]
        public void Location_Space()
        {
            Location locations = new Location();
            string expected = "                                    ";
            locations.Name = "                                    ";
            Assert.AreEqual(locations.Name, expected, "Locations Space is True");
        }
        [TestMethod]
        public void Location_Null()
        {
            Location locations = new Location();
            string expected = null;
            locations.Name = null;
            Assert.AreEqual(locations.Name, expected, "Locations Null is True");
        }
        [TestMethod]
        public void Location_0()
        {
            Location locations = new Location();
            string expected = "\0";
            locations.Name = "\0";
            Assert.AreEqual(locations.Name, expected, "Skills Null is True");
        }
    }

}
