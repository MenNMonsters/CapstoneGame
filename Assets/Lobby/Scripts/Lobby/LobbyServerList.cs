using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;

namespace Prototype.NetworkLobby
{
    public class LobbyServerList : MonoBehaviour
    {
        public LobbyManager lobbyManager;

        public RectTransform serverListRect;
        public GameObject serverEntryPrefab;
        public GameObject noServerFound;

        public GameObject passwordPromptPanel;
        public InputField passwordInput;
        public Button submitButton;

        protected int currentPage = 0;
        protected int previousPage = 0;

        static Color OddServerColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        static Color EvenServerColor = new Color(.94f, .94f, .94f, 1.0f);

        public InputField filterInputField;
        public Dropdown filterDropdown;
        public Dropdown privatefilterDown;

        void OnEnable()
        {
            currentPage = 0;
            previousPage = 0;

            filterInputField.onEndEdit.RemoveAllListeners();
            filterInputField.onEndEdit.AddListener(onEndEditField);

            filterDropdown.onValueChanged.AddListener(delegate
            {
                selectvalue(filterDropdown);
            });

            privatefilterDown.onValueChanged.AddListener(delegate
            {
                filterprivatevalue(privatefilterDown);
            });

            foreach (Transform t in serverListRect)
                Destroy(t.gameObject);

            noServerFound.SetActive(false);

            RequestPage(0);

            passwordPromptPanel.SetActive(false);
        }

        public void OnGUIMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            if (matches.Count == 0)
            {
                if (currentPage == 0)
                {
                    noServerFound.SetActive(true);
                }

                currentPage = previousPage;

                return;
            }

            noServerFound.SetActive(false);
            foreach (Transform t in serverListRect)
                Destroy(t.gameObject);

            for (int i = 0; i < matches.Count; ++i)
            {
                GameObject o = Instantiate(serverEntryPrefab) as GameObject;

                o.GetComponent<LobbyServerEntry>().Populate(matches[i], lobbyManager, (i % 2 == 0) ? OddServerColor : EvenServerColor, this);

                o.transform.SetParent(serverListRect, false);
            }
        }

        public void OnGUIMatchFilteredList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            for (int i = 0; i < matches.Count; ++i)
            {
                if (string.Equals(filterInputField.text, matches[i].name, System.StringComparison.OrdinalIgnoreCase))
                {
                    GameObject o = Instantiate(serverEntryPrefab) as GameObject;

                    o.GetComponent<LobbyServerEntry>().Populate(matches[i], lobbyManager, (i % 2 == 0) ? OddServerColor : EvenServerColor, this);

                    o.transform.SetParent(serverListRect, false);
                }

            }
        }

        public void OnGUIMatchFilterByPlayerNumberList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            for (int i = 0; i < matches.Count; ++i)
            {
                Debug.Log(matches[i].currentSize);
                if (filterDropdown.value == matches[i].currentSize)
                {
                    
                    GameObject o = Instantiate(serverEntryPrefab) as GameObject;

                    o.GetComponent<LobbyServerEntry>().Populate(matches[i], lobbyManager, (i % 2 == 0) ? OddServerColor : EvenServerColor, this);

                    o.transform.SetParent(serverListRect, false);
                }

            }
        }

        public void OnGUIMatchFilterByPrivatePublicList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            for (int i = 0; i < matches.Count; ++i)
            {
                if (privatefilterDown.value == 2)
                {

                    if (matches[i].isPrivate)
                    {
                        GameObject o = Instantiate(serverEntryPrefab) as GameObject;

                        o.GetComponent<LobbyServerEntry>().Populate(matches[i], lobbyManager, (i % 2 == 0) ? OddServerColor : EvenServerColor, this);

                        o.transform.SetParent(serverListRect, false);
                    }
                }else if(privatefilterDown.value == 1)
                {
                    if (!matches[i].isPrivate)
                    {
                        GameObject o = Instantiate(serverEntryPrefab) as GameObject;

                        o.GetComponent<LobbyServerEntry>().Populate(matches[i], lobbyManager, (i % 2 == 0) ? OddServerColor : EvenServerColor, this);

                        o.transform.SetParent(serverListRect, false);
                    }
                }

            }
        }

        public void ChangePage(int dir)
        {
            int newPage = Mathf.Max(0, currentPage + dir);

            //if we have no server currently displayed, need we need to refresh page0 first instead of trying to fetch any other page
            if (noServerFound.activeSelf)
                newPage = 0;

            RequestPage(newPage);
        }

        public void getResults()
        {
            if (!filterInputField.text.Equals(""))
            {
                foreach (Transform t in serverListRect)
                    Destroy(t.gameObject);
                lobbyManager.matchMaker.ListMatches(0, 6, "", false, 0, 0, OnGUIMatchFilteredList);
            }
            else
            {
                lobbyManager.matchMaker.ListMatches(0, 6, "", false, 0, 0, OnGUIMatchList);
            }
        }

        public void RequestPage(int page)
        {
            previousPage = currentPage;
            currentPage = page;
            lobbyManager.matchMaker.ListMatches(page, 6, "", false, 0, 0, OnGUIMatchList);
        }

        public void onEndEditField(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                getResults();
            }
        }

        private void selectvalue(Dropdown drpdown)
        {
            Debug.Log("selected: " + drpdown.value);
            if (drpdown.value != 0)
            {
                foreach (Transform t in serverListRect)
                    Destroy(t.gameObject);
                lobbyManager.matchMaker.ListMatches(0, 6, "", false, 0, 0, OnGUIMatchFilterByPlayerNumberList);
            }
            else
            {
                lobbyManager.matchMaker.ListMatches(0, 6, "", false, 0, 0, OnGUIMatchList);
            }
        }

        private void filterprivatevalue(Dropdown drpdown)
        {
            if (drpdown.value != 0)
            {
                foreach (Transform t in serverListRect)
                    Destroy(t.gameObject);
                lobbyManager.matchMaker.ListMatches(0, 6, "", false, 0, 0, OnGUIMatchFilterByPrivatePublicList);
            }
            else
            {
                lobbyManager.matchMaker.ListMatches(0, 6, "", false, 0, 0, OnGUIMatchList);
            }
        }

        public void showPasswordPromptPanel()
        {
            passwordPromptPanel.SetActive(true);
            passwordInput.Select();
            passwordInput.ActivateInputField();
        }

        public void onExitButtonClick()
        {
            passwordInput.text = "";
            passwordPromptPanel.SetActive(false);
        }

        void Destroy()
        {
            filterDropdown.onValueChanged.RemoveAllListeners();
            privatefilterDown.onValueChanged.RemoveAllListeners();
        }
    }
}